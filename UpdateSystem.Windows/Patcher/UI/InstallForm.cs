﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeElements.UpdateSystem.Windows.Patcher.UI
{
    internal partial class InstallForm : Form
    {
        private readonly UpdaterCore _updaterCore;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public InstallForm(UpdaterCore updaterCore)
        {
            InitializeComponent();
            headerLabel.ForeColor = Color.FromArgb(0, 51, 153);

            cancelButton.Text = updaterCore.Translation.Cancel;
            Text = updaterCore.Translation.ApplyUpdates;

            _updaterCore = updaterCore;
            _cancellationTokenSource = new CancellationTokenSource();
            updaterCore.PropertyChanged += UpdaterCoreOnPropertyChanged;

            Task.Run(() => updaterCore.Update(_cancellationTokenSource.Token));
        }

        private void UpdaterCoreOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(UpdaterCore.Progress):
                    InvokeIfRequired(() =>
                    {
                        progressLabel.Text = _updaterCore.Progress.ToString("P");
                        progressBar.Value = (int) (_updaterCore.Progress * 100);
                    });
                    break;
                case nameof(UpdaterCore.Status):
                    InvokeIfRequired(() => statusLabel.Text = _updaterCore.Status);
                    break;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InstallForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_updaterCore.IsShutdownRequested)
                return;

            e.Cancel = true; //is closed by the updater
            if (MessageBox.Show(this, _updaterCore.Translation.SureCancelUpdate, _updaterCore.Translation.Warning,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;

            _cancellationTokenSource.Cancel();
        }

        private void InvokeIfRequired(MethodInvoker action)
        {
            if (InvokeRequired)
                Invoke(action);
            else action();
        }
    }
}