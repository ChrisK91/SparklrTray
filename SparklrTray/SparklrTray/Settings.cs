using SparklrSharp.Sparklr;
using SparklrSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SparklrTray
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private async void signInButton_Click(object sender, EventArgs e)
        {
            if (PersistentProperties.LoggedIn)
            {
                await PersistentProperties.Conn.SignoffAsync();
                signInButton.Enabled = true;
                usernameTextbox.Enabled = true;
                passwordTextbox.Enabled = true;
                signInButton.Text = "Sign in";
                notificationUpdater.Enabled = false;
            }
            else
            {
                signInButton.Enabled = false;
                usernameTextbox.Enabled = false;
                passwordTextbox.Enabled = false;

                if (usernameTextbox.Text != String.Empty && passwordTextbox.Text != String.Empty)
                {
                    bool success = await PersistentProperties.Conn.SigninAsync(usernameTextbox.Text, passwordTextbox.Text);

                    if (success)
                    {
                        PersistentProperties.LoggedIn = true;
                        signInButton.Text = "Sign out";
                        notificationUpdater.Enabled = true;
                        signInButton.Enabled = true;
                    }
                    else
                    {
                        signInButton.Enabled = true;
                        usernameTextbox.Enabled = true;
                        passwordTextbox.Enabled = true;
                    }
                }
                else
                {
                    signInButton.Enabled = true;
                    usernameTextbox.Enabled = true;
                    passwordTextbox.Enabled = true;
                }
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            Show();
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
        }

        private async void notificationUpdater_Tick(object sender, EventArgs e)
        {
            notificationUpdater.Enabled = false;

            List<Notification> notifications = new List<Notification>(await PersistentProperties.Conn.GetNotificationsAsync());

            if (notifications.Count > 1)
            {
                notifyIcon.Text = String.Format("You have {0} notifications.", notifications.Count);
            }
            else if (notifications.Count > 0)
            {
                notifyIcon.Text = String.Format("You have one notification.");
            }
            else
            {
                notifyIcon.Text = "No notifications";
            }

            List<Notification> copy = new List<Notification>(notifications);

            foreach (Notification n in copy)
            {
                if (PersistentProperties.ShownIDs.Contains(n.Id))
                    notifications.Remove(n);
            }

            if (notifications.Count == 1)
                notifyIcon.ShowBalloonTip(10000, "sparklr*", notifications[0].NotificationText, ToolTipIcon.None);
            else if (notifications.Count > 1)
                notifyIcon.ShowBalloonTip(10000, "sparklr*", String.Format("You have {0} new notifications.", notifications.Count), ToolTipIcon.None);

            foreach (Notification n in notifications)
                PersistentProperties.ShownIDs.Add(n.Id);

            if (PersistentProperties.LoggedIn)
                notificationUpdater.Enabled = true;
        }
    }
}
