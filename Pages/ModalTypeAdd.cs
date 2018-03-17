﻿using System;
using System.Web.UI.WebControls;
using SS.GovInteract.Core;
using SS.GovInteract.Model;

namespace SS.GovInteract.Pages
{
    public class ModalTypeAdd : PageBase
    {
        public Literal LtlMessage;
        public TextBox TbTypeName;

        private int _channelId;
        private int _id;

        public static string GetOpenWindowStringToAdd(int siteId, int channelId)
        {
            return Utils.GetOpenLayerString("添加办件类型", $"{nameof(ModalTypeAdd)}.aspx?siteId={siteId}&channelId={channelId}", 450, 350);
        }

        public static string GetOpenWindowStringToEdit(int siteId, int channelId, int id)
        {
            return Utils.GetOpenLayerString("修改办件类型", $"{nameof(ModalTypeAdd)}.aspx?siteId={siteId}&channelId={channelId}&id={id}", 450, 350);
        }

        public void Page_Load(object sender, EventArgs e)
        {  
            _channelId = Utils.ToInt(Request.QueryString["channelId"]);
            _id = Utils.ToInt(Request.QueryString["id"]);

            if (!IsPostBack)
            {
                if (_id > 0)
                {
                    var typeInfo = Main.TypeDao.GetTypeInfo(_id);
                    if (typeInfo != null)
                    {
                        TbTypeName.Text = typeInfo.TypeName;
                    }
                } 
            }
        }

        public void Submit_OnClick(object sender, EventArgs e)
        { 
            TypeInfo typeInfo = null; 
            if (_id > 0)
            {
                try
                {
                    typeInfo = Main.TypeDao.GetTypeInfo(_id);
                    if (typeInfo != null)
                    {
                        if (typeInfo.TypeName == TbTypeName.Text)
                        {
                            LtlMessage.Text = Utils.GetMessageHtml("办件类型名称不能与原来相同！", false); 
                        }
                        else
                        { 
                            var typeNameArrayList = Main.TypeDao.GetTypeNameArrayList(_channelId);
                            if (typeNameArrayList.IndexOf(TbTypeName.Text) != -1)
                            {
                                LtlMessage.Text = Utils.GetMessageHtml($"办件类型添加失败，办件类型名称已存在！", false);
                            }
                            else
                            {
                                typeInfo.TypeName = TbTypeName.Text;
                                Main.TypeDao.Update(typeInfo);
                                LtlMessage.Text = Utils.GetMessageHtml("办件类型修改成功！", true);
                                Utils.CloseModalPage(Page);
                            }
                        }  
                    } 
                }
                catch (Exception ex)
                {
                    LtlMessage.Text = Utils.GetMessageHtml($"办件类型修改失败，{ex.Message}", false);
                } 
                
            }
            else
            {
                var typeNameArrayList = Main.TypeDao.GetTypeNameArrayList(_channelId);
                if (typeNameArrayList.IndexOf(TbTypeName.Text) != -1)
                {
                    LtlMessage.Text = Utils.GetMessageHtml($"办件类型添加失败，办件类型名称已存在！", false); 
                }
                else
                {
                    try
                    {
                        typeInfo = new TypeInfo(0, TbTypeName.Text, _channelId, SiteId, 0); 
                        Main.TypeDao.Insert(typeInfo);
                        LtlMessage.Text = Utils.GetMessageHtml("办件类型添加成功！", true);
                        Utils.CloseModalPage(Page);
                    }
                    catch (Exception ex)
                    {
                        LtlMessage.Text = Utils.GetMessageHtml($"办件类型添加失败，{ex.Message}", false); 
                    }
                }
            }  
        }
    }
}
