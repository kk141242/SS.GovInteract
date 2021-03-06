﻿<%@ Page Language="C#" Inherits="SS.GovInteract.Pages.PageListAccept" %>
  <%@ Register TagPrefix="ctrl" Namespace="SS.GovInteract.Controls" Assembly="SS.GovInteract" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
      <link href="assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
      <link href="assets/css/siteserver.min.css" rel="stylesheet" type="text/css" />
      <script src="assets/js/jquery.min.js" type="text/javascript"></script>
      <script src="assets/sweetalert/sweetalert.min.js" type="text/javascript"></script>
      <script src="assets/layer/layer.min.js" type="text/javascript"></script>

      <script>
        function loopRows(oTable, callBack) {
          if (!oTable) return;
          callBack = callBack || function () {};
          var trs = oTable.rows;
          var i = 0,
            l = trs.length;
          var flag = i < l;

          while (flag ? i < l : i > l) {
            var cur = trs[i];
            try {
              callBack(cur, i);
            } catch (e) {
              if (e == 'break') {
                break;
              }
            }
            flag ? i++ : i--;
          }
        }

        function selectRows(layer, bcheck) {
          for (var i = 0; i < layer.childNodes.length; i++) {
            if (layer.childNodes[i].childNodes.length > 0) {
              selectRows(layer.childNodes[i], bcheck);
            } else {
              if (layer.childNodes[i].type == "checkbox") {
                layer.childNodes[i].checked = bcheck;
                var cb = $(layer.childNodes[i]);
                var tr = cb.closest('tr');
                if (!tr.hasClass("thead")) {
                  cb.is(':checked') ? tr.addClass('table-active') : tr.removeClass('table-active');
                }
              }
            }
          }
        }

        function chkSelect(e) {
          var e = (e || event);
          var el = this;
          if (el.getElementsByTagName('input') && el.getElementsByTagName('input').length > 0) {
            if ($(el).hasClass('thead')) return;
            el.className = (el.className == 'table-active' ? '' : 'table-active');
            el.getElementsByTagName('input')[0].checked = (el.className == 'table-active');
          }
        }
        $(document).ready(function () {
          loopRows(document.getElementById('contents'), function (cur) {
            cur.onclick = chkSelect;
          });
        });
      </script>
    </head>

    <body>
      <form class="m-l-15 m-r-15 m-t-15" runat="server">

        <div class="card-box">
          <h4 class="m-t-0 m-b-20 header-title">
            待受理办件(办件数：
            <asp:Literal id="LtlTotalCount" runat="server" />)
          </h4>

          <!-- <div class="btn-toolbar mt-4 mb-4" role="toolbar">
            <div class="btn-group">

              <asp:PlaceHolder id="PhAccept" runat="server">
                <asp:HyperLink id="HlAccept" class="btn btn-light text-secondary" NavigateUrl="javascript:;" runat="server" Text="受 理"></asp:HyperLink>
                <asp:HyperLink id="HlDeny" class="btn btn-light text-secondary" NavigateUrl="javascript:;" runat="server" Text="拒 绝"></asp:HyperLink>
              </asp:PlaceHolder>
              <asp:PlaceHolder id="PhSwitchToTranslate" runat="server">
                <asp:HyperLink id="HlSwitchTo" class="btn btn-light text-secondary" NavigateUrl="javascript:;" runat="server" Text="转 办"></asp:HyperLink>
                <asp:HyperLink id="HlTranslate" class="btn btn-light text-secondary" NavigateUrl="javascript:;" runat="server" Text="转 移"></asp:HyperLink>
              </asp:PlaceHolder>
              <asp:PlaceHolder id="PhComment" runat="server">
                <asp:HyperLink id="HlComment" class="btn btn-light text-secondary" NavigateUrl="javascript:;" runat="server" Text="批 示"></asp:HyperLink>
              </asp:PlaceHolder>
              <asp:PlaceHolder id="PhDelete" runat="server">
                <asp:HyperLink id="HlDelete" class="btn btn-light text-secondary" NavigateUrl="javascript:;" runat="server" Text="删 除"></asp:HyperLink>
              </asp:PlaceHolder>

            </div>
          </div> -->

          <div class="table-responsive" data-pattern="priority-columns">
            <table id="contents" class="table">
              <thead>
                <tr>
                  <th class="text-center">编号</th>
                  <th>办件标题(点击进入操作界面) </th>
                  <th class="text-center">提交日期</th>
                  <!-- <th>意见</th> -->
                  <th class="text-center">办理部门</th>
                  <th class="text-center">期限</th>
                  <th class="text-center">状态</th>
                  <th class="text-center"></th>
                  <th width="20" class="text-center">
                    <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                  </th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <asp:Literal id="ltlTr" runat="server"></asp:Literal>
                    <td class="text-center">
                      <asp:Literal ID="ltlID" runat="server"></asp:Literal>
                    </td>
                    <td>
                      <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                    </td>
                    <td class="text-center">
                      <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                    </td>
                    <!-- <td>
                      <asp:Literal ID="ltlRemark" runat="server"></asp:Literal>
                    </td> -->
                    <td class="text-center">
                      <asp:Literal ID="ltlDepartment" runat="server"></asp:Literal>
                    </td>
                    <td class="text-center">
                      <asp:Literal ID="ltlLimit" runat="server"></asp:Literal>
                    </td>
                    <td class="text-center">
                      <asp:Literal ID="ltlState" runat="server"></asp:Literal>
                    </td>
                    <td class="text-center">
                      <asp:Literal ID="ltlFlowUrl" runat="server"></asp:Literal>
                      <asp:Literal ID="ltlViewUrl" runat="server"></asp:Literal>
                      <asp:Literal ID="ltlReplyUrl" runat="server"></asp:Literal>
                      <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
                    </td>
                    <td class="text-center">
                      <input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
                    </td>
                    </tr>
                  </itemtemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </div>

          <ctrl:SqlPager id="SpContents" runat="server" class="table table-pager" />

        </div>

      </form>
    </body>

    </html>