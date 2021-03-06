﻿using System.Web;

namespace SS.GovInteract.Pages
{
	public class PageContentReply : PageBaseContent
    {
        public static string GetRedirectUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return $"{nameof(PageContentReply)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        }
	}
}
