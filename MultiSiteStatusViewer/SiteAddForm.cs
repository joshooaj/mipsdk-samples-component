﻿using System;
using System.Net;
using System.Windows.Forms;
using VideoOS.Platform;

namespace MultiSiteStatusViewer
{
	/// <summary>
	/// This form lets a user select a specific XProtect server (via URI), and get the servers SiteInfo information.
	/// Based on this information the use can then add the entire site-hierarchy or any specific server.
	/// </summary>
	public partial class SiteAddForm : Form
	{
		private CredentialCache _credentialCache = null;

		public SiteAddForm()
		{
			InitializeComponent();
		}

		#region Properties to be used when adding server(s)

		internal bool IncludeChildSites
		{
			get { return checkBoxChildren.Checked; }
		}

		internal Item SelectedSiteItem
		{
			get
			{
				if (treeViewSites.SelectedNode!=null)
				{
					return treeViewSites.SelectedNode.Tag as Item;
				}
				return null;
			}
		}

		internal CredentialCache CredentialCache
		{
			get
			{
				// Very important: Include ALL sites in the Credential Cache, in order to be allowed to access the sites.
				AddUriToCache(_credentialCache, SelectedSiteItem);
				return _credentialCache;
			}
		}

		internal bool SecureOnly
        {
			get { return secureOnlyCheckBox.Checked; }
        }

		// Add a given URI to the CredentialCache
		private void AddUriToCache(CredentialCache cc, Item item)
		{
			String authorization = radioButtonBasic.Checked ? "Basic" : "Negotiate";

			try
			{
				if (radioButtonCurrent.Checked)
					cc.Add(item.FQID.ServerId.Uri, authorization, CredentialCache.DefaultNetworkCredentials);
				else
				{
					string[] parts = textBoxUsername.Text.Split('\\');
					if (parts.Length==1)
						cc.Add(item.FQID.ServerId.Uri, authorization, new NetworkCredential(textBoxUsername.Text, textBoxPassword.Text));
					else
						cc.Add(item.FQID.ServerId.Uri, authorization, new NetworkCredential(parts[1], textBoxPassword.Text, parts[0]));

				}
			} catch (Exception)
			{
				// Ignore Duplicate adding
			}

			if (IncludeChildSites)
			{
				foreach (Item child in item.GetChildren())
				{
					AddUriToCache(cc, child);
				}
			}
		}

		#endregion

		#region User Click Handling

		private void buttonOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void radioButtonCurrent_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonCurrent.Checked)
			{
				textBoxUsername.Text = Environment.UserDomainName + "\\" + Environment.UserName;
				textBoxPassword.Text = "";

				textBoxUsername.Enabled = false;
				textBoxPassword.Enabled = false;
			} else
			{
				textBoxUsername.Enabled = true;
				textBoxPassword.Enabled = true;				
			}
		}

		private void buttonValidate_Click(object sender, EventArgs e)
		{
            if (textBoxServer.Text.StartsWith("http://") == false && textBoxServer.Text.StartsWith("https://") == false)
				textBoxServer.Text = "http://" + textBoxServer.Text;
			Uri uri = new Uri(textBoxServer.Text); //TODO you get exception if the url is not an url--
			String authorization = radioButtonBasic.Checked ? "Basic" : "Negotiate";
			String username = radioButtonCurrent.Checked ? "" : textBoxUsername.Text;
			_credentialCache = VideoOS.Platform.Login.Util.BuildCredentialCache(uri, username, textBoxPassword.Text,
			                                                                      authorization);
			Item siteItem = VideoOS.Platform.SDK.Environment.LoadSiteItem(secureOnlyCheckBox.Checked, uri, _credentialCache);

			treeViewSites.Nodes.Clear();
			if (siteItem==null)
			{
				treeViewSites.Nodes.Add("Unable to contact server with given credentials");
				buttonOK.Enabled = false;
			}
			else
			{
				TreeNode tn = treeViewSites.Nodes.Add(siteItem.Name);
				tn.Tag = siteItem;
				if (siteItem.HasChildren != VideoOS.Platform.HasChildren.No)
				{
					foreach (Item childSite in siteItem.GetChildren())
					{
						AddChildSite(childSite, tn);
					}
				}
			}
		}

		private void AfterSelect(object sender, TreeViewEventArgs e)
		{
			buttonOK.Enabled = true;
		}

		#endregion

		#region Class private methods

        /// <summary>
        /// I made this non-static!
        /// </summary>
        /// <param name="siteItem"></param>
        /// <param name="treeNode"></param>
		private void AddChildSite(Item siteItem, TreeNode treeNode)
        {
            AddUriToCache(_credentialCache, siteItem);
            //Item siteItemCheck = VideoOS.Platform.SDK.Environment.LoadSiteItem(siteItem.FQID.ServerId.Uri, CredentialCache);
            //if (siteItemCheck != null)
            //{
                TreeNode tn = treeNode.Nodes.Add(siteItem.Name);
                tn.Tag = siteItem;
			    
                if (siteItem.HasChildren != VideoOS.Platform.HasChildren.No)
                {
                    foreach (Item childSite in siteItem.GetChildren())
                    {
                        AddChildSite(childSite, tn);
                    }
                }    
            //}
            //else
            //{
            //    treeNode.Nodes.Add("Unable to contact child server with given credentials");    
            //}
            
		}

		#endregion


	}
}
