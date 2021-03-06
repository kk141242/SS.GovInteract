﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.GovInteract.Core;
using SS.GovInteract.Model;
using SS.GovInteract.Provider;

namespace SS.GovInteract.Pages
{
	public class ModalApplyRedo : PageBase
    {
        public Literal LtlMessage;

        protected TextBox tbRedoRemark;
        public Literal ltlDepartmentName;
        public Literal ltlUserName;

        private int _channelId;
        private List<int> _idArrayList;
        private IAdministratorInfo _adminInfo;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScript("要求返工", $"{nameof(ModalApplyRedo)}.aspx?siteId={siteId}&channelId={channelId}", 450, 320);
        }
        
		public void Page_Load(object sender, EventArgs e)
		{
		    _channelId = Utils.ToInt(Request.QueryString["channelId"]);
            _idArrayList = Utils.StringCollectionToIntList(Request.QueryString["IDCollection"]);

		    _adminInfo = Main.AdminApi.GetAdminInfoByUserId(AuthRequest.AdminId);

            if (!IsPostBack)
			{
                ltlDepartmentName.Text = DepartmentManager.GetDepartmentName(_adminInfo.DepartmentId);
                ltlUserName.Text = _adminInfo.DisplayName;
			}
		}

        public void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
				
            try
            {
                if (string.IsNullOrEmpty(tbRedoRemark.Text))
                {
                    LtlMessage.Text = Utils.GetMessageHtml("要求返工失败，必须填写意见！", false);
                    return;
                }

                foreach (int contentID in _idArrayList)
                {
                    var contentInfo = Main.ContentApi.GetContentInfo(SiteId, _channelId, contentID);
                    var state = EStateUtils.GetEnumType(contentInfo.GetString(ContentAttribute.State));

                    if (state == EState.Replied || state == EState.Redo)
                    {
                        var remarkInfo = new RemarkInfo(0, SiteId, contentInfo.ChannelId, contentInfo.Id, ERemarkTypeUtils.GetValue(ERemarkType.Redo), tbRedoRemark.Text, _adminInfo.DepartmentId, AuthRequest.AdminName, DateTime.Now);
                        RemarkDao.Insert(remarkInfo);

                        ApplyManager.Log(SiteId, contentInfo.ChannelId, contentID, ELogTypeUtils.GetValue(ELogType.Redo), AuthRequest.AdminName, _adminInfo.DepartmentId);
                        contentInfo.Set(ContentAttribute.State, EStateUtils.GetValue(EState.Redo));
                        Main.ContentApi.Update(SiteId, contentInfo.ChannelId, contentInfo);
                    }
                }

                isChanged = true;
            }
			catch(Exception ex)
			{
                LtlMessage.Text = Utils.GetMessageHtml(ex.Message, false);
                isChanged = false;
			}

			if (isChanged)
			{
                LayerUtils.Close(Page);
			}
		}

	}
}
