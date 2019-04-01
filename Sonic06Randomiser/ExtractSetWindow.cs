﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sonic06Randomiser
{
    public partial class ExtractSetWindow : Form
    {
        #region Main Variables
        bool extractFolder = false;
        string filepath = "";
        int outputFolderType = 0; //0 = Custom, 1 = Source, 2 = Program
        string output = "";
        #endregion

        public ExtractSetWindow()
        {
            InitializeComponent();
        }

        private void folderRandom_CheckedChanged(object sender, EventArgs e)
        {
            filepathBox.Text = "";
            filepath = "";
            if (folderRandom.Checked)
            {
                filepathLabel.Text = "Folder to Extract:";
                extractFolder = true;
            }
            if (!folderRandom.Checked)
            {
                filepathLabel.Text = "SET to Extract:";
                extractFolder = false;
            }
        }

        #region Directory Radio Buttons
        private void customDirButton_CheckedChanged(object sender, EventArgs e)
        {
            if (customDirButton.Checked)
            {
                outputFolderType = 0;
                outputBox.Enabled = true;
                outputButton.Enabled = true;
            }
        }

        private void sourceDirButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sourceDirButton.Checked)
            {
                outputFolderType = 1;
                outputBox.Enabled = false;
                outputButton.Enabled = false;
            }
        }

        private void programDirButton_CheckedChanged(object sender, EventArgs e)
        {
            if (programDirButton.Checked)
            {
                outputFolderType = 2;
                outputBox.Enabled = false;
                outputButton.Enabled = false;
            }
        }
        #endregion

        private void filepathBox_TextChanged(object sender, EventArgs e)
        {
            filepath = filepathBox.Text;
            if (filepathBox.Text == "")
            {
                extractButton.Enabled = false;
            }
            else
            {
                extractButton.Enabled = true;
            }
        }

        private void filepathButton_Click(object sender, EventArgs e)
        {
            if (!extractFolder)
            {
                OpenFileDialog xmlBrowser = new OpenFileDialog();
                xmlBrowser.Title = "Select SET";
                xmlBrowser.Filter = "SONIC THE HEDGEHOG (2006) SET file (*.set)|*.set|All files (*.*)|*.*";
                xmlBrowser.FilterIndex = 1;
                xmlBrowser.RestoreDirectory = true;
                if (xmlBrowser.ShowDialog() == DialogResult.OK)
                {
                    filepath = xmlBrowser.FileName;
                    filepathBox.Text = filepath;
                }
            }
            else
            {
                FolderBrowserDialog outputBrowser = new FolderBrowserDialog();
                if (outputBrowser.ShowDialog() == DialogResult.OK)
                {
                    filepath = outputBrowser.SelectedPath;
                    filepathBox.Text = filepath;
                }
            }
        }

        private void outputBox_TextChanged(object sender, EventArgs e)
        {
            output = outputBox.Text;
        }

        private void outputButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog outputBrowser = new FolderBrowserDialog();
            if (outputBrowser.ShowDialog() == DialogResult.OK)
            {
                output = outputBrowser.SelectedPath;
                outputBox.Text = output;
            }
        }

        private void extractButton_Click(object sender, EventArgs e)
        {
            SetClass.Extract(extractFolder, outputFolderType, output, filepath);
        }
    }
}
