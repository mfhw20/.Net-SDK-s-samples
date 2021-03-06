﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VisioForge.Controls.UI.WinForms;
using VisioForge.Tools.MediaInfo;
using VisioForge.Types;

namespace Multiple_Video_Streams_Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                edFilenameOrURL.Text = openFileDialog1.FileName;
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Debug_Mode = cbDebugMode.Checked;
            
            mmLog.Clear();

            MediaPlayer1.Video_Renderer.Zoom_Ratio = 0;
            MediaPlayer1.Video_Renderer.Zoom_ShiftX = 0;
            MediaPlayer1.Video_Renderer.Zoom_ShiftY = 0;

            var info = new MediaInfoReader();
            info.Filename = edFilenameOrURL.Text;
            info.ReadFileInfo(true);

            MediaPlayer1.Multiple_Video_Streams_Mappings_Clear();
            if (info.Video_Streams_Count() > 1)
            {
                for (int i = 0; i < info.Video_Streams_Count() - 1; i++)
                {
                    if (i > 3)
                    {
                        break;
                    }

                    Panel panel = null;
                    switch (i)
                    {
                        case 0:
                            panel = pnScreen1;
                            break;
                        case 1:
                            panel = pnScreen2;
                            break;
                        case 2:
                            panel = pnScreen3;
                            break;
                        case 3:
                            panel = pnScreen4;
                            break;
                    }

                    if (panel != null)
                    {
                        MediaPlayer1.Multiple_Video_Streams_Mappings_Add(i, panel.Handle, panel.Width, panel.Height);
                    }
                }
            }

            MediaPlayer1.FilenamesOrURL.Clear();
            MediaPlayer1.FilenamesOrURL.Add(edFilenameOrURL.Text);

            MediaPlayer1.Audio_PlayAudio = true;

            MediaPlayer1.Source_Mode = VFMediaPlayerSource.File_DS;
            
            if (MediaPlayer1.Filter_Supported_EVR())
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.EVR;
            }
            else if (MediaPlayer1.Filter_Supported_VMR9())
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.VMR9;
            }
            else
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.VideoRenderer;
            }

            MediaPlayer1.Video_Sample_Grabber_UseForVideoEffects = true;

            MediaPlayer1.Play();
            
            timer1.Enabled = true;
        }

        private void MediaPlayer1_OnError(object sender, ErrorsEventArgs e)
        {
            mmLog.Text = mmLog.Text + e.Message + Environment.NewLine;
        }

        private void btResume_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Resume();
        }

        private void btPause_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Pause();
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Stop();
            timer1.Enabled = false;
            tbTimeline.Value = 0;
        }

        private void btNextFrame_Click(object sender, EventArgs e)
        {
            MediaPlayer1.NextFrame();
        }

        private void tbTimeline_Scroll(object sender, EventArgs e)
        {
            if (Convert.ToInt32(timer1.Tag) == 0)
            {
                MediaPlayer1.Position_Set_Time(tbTimeline.Value * 1000);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Tag = 1;
            tbTimeline.Maximum = (int)(MediaPlayer1.Duration_Time() / 1000.0);

            int value = (int)(MediaPlayer1.Position_Get_Time() / 1000.0);
            if ((value > 0) && (value < tbTimeline.Maximum))
            {
                tbTimeline.Value = value;
            }

            lbTime.Text = MediaPlayer.Helpful_SecondsToTimeFormatted(tbTimeline.Value) + "/" + MediaPlayer.Helpful_SecondsToTimeFormatted(tbTimeline.Maximum);

            timer1.Tag = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void MediaPlayer1_OnStop(object sender, StopEventArgs e)
        {
            BeginInvoke(new StopDelegate(StopDelegateMethod), null);
        }

        private delegate void StopDelegate();

        private void StopDelegateMethod()
        {
            tbTimeline.Value = 0;
        }
    }
}
