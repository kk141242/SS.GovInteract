﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.GovInteract.Core;
using SS.GovInteract.Model;

namespace SS.GovInteract.Pages
{
    public class PageBase : Page
    {
        public Literal LtlMessage;

        public int SiteId { get; private set; }

        public List<IChannelInfo> ChannelInfoList { get; private set; }

        public IRequest AuthRequest { get; private set; }

        private ConfigInfo _configInfo;

        public ConfigInfo ConfigInfo => _configInfo ?? (_configInfo = Main.Instance.GetConfigInfo(SiteId));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AuthRequest = Main.Instance.PluginApi.AuthRequest(Request);

            SiteId = Convert.ToInt32(Request.QueryString["siteId"]);

            if (!Main.Instance.AdminApi.IsSiteAuthorized(SiteId))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
            }

            ChannelInfoList = InteractManager.GetInteractChannelInfoList(SiteId);

            if (ChannelInfoList.Count == 0)
            {
                HttpContext.Current.Response.Redirect(PageInit.GetRedirectUrl(SiteId, Request.RawUrl));
                HttpContext.Current.Response.End();
            }
        }
    }
}