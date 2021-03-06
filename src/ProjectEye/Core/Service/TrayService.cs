﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 管理和显示托盘图标
    /// </summary>
    public class TrayService : IService
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.ComponentModel.IContainer components;
        private readonly App app;
        private readonly MainService mainService;
        private readonly ConfigService config;
        //托盘菜单项
        private System.Windows.Forms.MenuItem menuItem_norest;
        private System.Windows.Forms.MenuItem menuItem_sound;
        public TrayService(App app, MainService mainService, ConfigService config)
        {
            this.app = app;
            this.mainService = mainService;
            this.config = config;
            this.config.Changed += new EventHandler(config_Changed);

            components = new System.ComponentModel.Container();
            contextMenu = new System.Windows.Forms.ContextMenu();
            //不要提醒我
            menuItem_norest = new System.Windows.Forms.MenuItem();
            menuItem_norest.Text = "不要提醒我";
            menuItem_norest.Click += new System.EventHandler(menuItem_norest_Click);
            //声音提示
            menuItem_sound = new System.Windows.Forms.MenuItem();
            menuItem_sound.Checked = config.options.general.sound;
            menuItem_sound.Text = "提示音";
            menuItem_sound.Click += new System.EventHandler(menuItem_sound_Click);
            //退出菜单项
            System.Windows.Forms.MenuItem menuItem_exit = new System.Windows.Forms.MenuItem();
            menuItem_exit.Text = "退出";
            menuItem_exit.Click += new System.EventHandler(menuItem_exit_Click);

            //统计数据
            System.Windows.Forms.MenuItem menuItem_data = new System.Windows.Forms.MenuItem();
            menuItem_data.Text = "统计";
            //menuItem_data.Click += new System.EventHandler(menuItem_data_Click);



            //选项
            System.Windows.Forms.MenuItem menuItem_options = new System.Windows.Forms.MenuItem();
            menuItem_options.Text = "选项";
            menuItem_options.Click += new EventHandler(menuItem_options_Click);





            //contextMenu.MenuItems.Add(menuItem_data);
            //contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(menuItem_norest);
            contextMenu.MenuItems.Add(menuItem_sound);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(menuItem_options);
            //contextMenu.MenuItems.Add(menuItem_update);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(menuItem_exit);

            //this.contextMenu.MenuItems.AddRange(
            //            new System.Windows.Forms.MenuItem[] { menuItem_norest, menuItem_sound, menuItem_exit });


            notifyIcon = new System.Windows.Forms.NotifyIcon(components);
            UpdateIcon("sunglasses");
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Text = "Project Eye";
            notifyIcon.Visible = true;
            //notifyIcon.DoubleClick += new System.EventHandler(notifyIcon_DoubleClick);

            app.Exit += new ExitEventHandler(app_Exit);
        }

        private void config_Changed(object sender, EventArgs e)
        {
            menuItem_norest.Checked = config.options.general.noreset;
            menuItem_sound.Checked = config.options.general.sound;

        }

        private void menuItem_options_Click(object sender, EventArgs e)
        {
            WindowManager.CreateWindowInScreen("OptionsWindow");
            WindowManager.Show("OptionsWindow");
        }



        private void menuItem_sound_Click(object sender, EventArgs e)
        {
            var item = sender as System.Windows.Forms.MenuItem;
            item.Checked = !item.Checked;
            config.options.general.sound = item.Checked;
            config.Save();
        }

        public void Init()
        {

        }

        private void menuItem_norest_Click(object sender, EventArgs e)
        {
            var item = sender as System.Windows.Forms.MenuItem;
            item.Checked = !item.Checked;
            config.options.general.noreset = item.Checked;
            if (item.Checked)
            {
                //不要提醒
                UpdateIcon("dizzy");
                mainService.Pause();
            }
            else
            {
                //继续
                UpdateIcon("sunglasses");
                mainService.Start();

            }
        }


        private void Remove()
        {
            notifyIcon.Visible = false;
            if (components != null)
            {
                components.Dispose();
            }
        }
        private void UpdateIcon(string name)
        {
            Uri iconUri = new Uri("/ProjectEye;component/Resources/" + name + ".ico", UriKind.RelativeOrAbsolute);
            StreamResourceInfo info = Application.GetResourceStream(iconUri);
            notifyIcon.Icon = new Icon(info.Stream);
        }
        private void menuItem_exit_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void app_Exit(object sender, ExitEventArgs e)
        {
            mainService.Exit();
            Remove();
        }



    }
}
