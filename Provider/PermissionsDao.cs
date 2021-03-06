﻿using System.Collections.Generic;
using System.Data; 
using SiteServer.Plugin;
using SS.GovInteract.Core;
using SS.GovInteract.Model;

namespace SS.GovInteract.Provider
{
    public static class PermissionsDao
    {
        public const string TableName = "ss_govinteract_permissions"; 

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(PermissionsInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(PermissionsInfo.UserName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(PermissionsInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PermissionsInfo.Permissions),
                DataType = DataType.Text 
            }  
        };

        public static void Insert(int siteId, PermissionsInfo permissionsInfo)
        { 
            string sqlString = $@"INSERT INTO {TableName}
            (
                {nameof(PermissionsInfo.UserName)}, 
                {nameof(PermissionsInfo.ChannelId)}, 
                {nameof(PermissionsInfo.Permissions)} 
            ) VALUES (
                @{nameof(PermissionsInfo.UserName)}, 
                @{nameof(PermissionsInfo.ChannelId)}, 
                @{nameof(PermissionsInfo.Permissions)} 
            )";

            var parameters = new[]
            {
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.UserName), permissionsInfo.UserName),
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.ChannelId), permissionsInfo.ChannelId),
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.Permissions), permissionsInfo.Permissions)
            };

            if (!ChannelDao.IsExists(permissionsInfo.ChannelId))
            {
                var channelInfo = new ChannelInfo(0, permissionsInfo.ChannelId, siteId, 0, 0, string.Empty, string.Empty);
                ChannelDao.Insert(channelInfo);
            }

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters);
        }

        public static void Delete(string userName, int channelId)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(PermissionsInfo.UserName)} = @{nameof(PermissionsInfo.UserName)} AND {nameof(PermissionsInfo.ChannelId)} = @{nameof(PermissionsInfo.ChannelId)}";

            var parameters = new[]
            {
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.UserName), userName),
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.ChannelId), channelId)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters);
        }

        public static void Update(PermissionsInfo permissionsInfo)
        {
            string sqlString = $"UPDATE {TableName} SET {nameof(PermissionsInfo.Permissions)} = @{nameof(PermissionsInfo.Permissions)} WHERE {nameof(PermissionsInfo.Id)} = @{nameof(PermissionsInfo.Id)}";

            var parameters = new[]
            {
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.Permissions), permissionsInfo.Permissions),
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.Id), permissionsInfo.Id)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters);
        }

        public static PermissionsInfo GetPermissionsInfo(string userName, int channelId)
        {
            PermissionsInfo permissionsInfo = null;

            string sqlString = $"SELECT {nameof(PermissionsInfo.Id)}, {nameof(PermissionsInfo.UserName)}, {nameof(PermissionsInfo.ChannelId)}, {nameof(PermissionsInfo.Permissions)} FROM {TableName} WHERE {nameof(PermissionsInfo.UserName)} = @{nameof(PermissionsInfo.UserName)} AND {nameof(PermissionsInfo.ChannelId)} = @{nameof(PermissionsInfo.ChannelId)}";

            var parameters = new[]
            {
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.UserName), userName),
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.ChannelId), channelId)
            };

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters))
            {
                if (rdr.Read())
                {
                    permissionsInfo = GetPermissionsInfo(rdr);
                }
                rdr.Close();
            } 

            return permissionsInfo;
        }

        public static List<PermissionsInfo> GetPermissionsInfoList(string userName)
        {
            var list = new List<PermissionsInfo>();

            var parameters = new[]
           {
                Context.DatabaseApi.GetParameter(nameof(PermissionsInfo.UserName), userName) 
           };

            string sqlString = $"SELECT {nameof(PermissionsInfo.Id)}, {nameof(PermissionsInfo.UserName)}, {nameof(PermissionsInfo.ChannelId)}, {nameof(PermissionsInfo.Permissions)} FROM {TableName} WHERE {nameof(PermissionsInfo.UserName)} = @{nameof(PermissionsInfo.UserName)}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters))
            {
                while (rdr.Read())
                {
                    var permissionsInfo = GetPermissionsInfo(rdr);
                    list.Add(permissionsInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public static Dictionary<int, List<string>> GetPermissionSortedList(string userName)
        {
            var sortedlist = new Dictionary<int, List<string>>();

            var permissionsInfoList = GetPermissionsInfoList(userName);

            foreach (var permissionsInfo in permissionsInfoList)
            {
                var list = new List<string>();
                if (sortedlist[permissionsInfo.ChannelId] != null)
                {
                    list = sortedlist[permissionsInfo.ChannelId];
                }

                var permissionList = Utils.StringCollectionToStringList(permissionsInfo.Permissions);
                foreach (var permission in permissionList)
                {
                    if (!list.Contains(permission)) list.Add(permission);
                }
                sortedlist[permissionsInfo.ChannelId] = list;
            }

            return sortedlist;
        }

        private static PermissionsInfo GetPermissionsInfo(IDataReader rdr)
        {
            if (rdr == null) return null;
            var i = 0;

            return new PermissionsInfo
            {
                Id = Context.DatabaseApi.GetInt(rdr, i++),
                UserName = Context.DatabaseApi.GetString(rdr, i++),
                ChannelId = Context.DatabaseApi.GetInt(rdr, i++),
                Permissions = Context.DatabaseApi.GetString(rdr, i) 
            };
        }
    }
}
