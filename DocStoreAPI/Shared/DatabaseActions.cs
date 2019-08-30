using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DocStore.API.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DocStore.API.Shared
{
    public class DatabaseActions
    {
        public static bool CheckLoginKey(string apiAuthKey)
        {
            string query = "SELECT ExpiresAt FROM dbo.SecurityKeys WHERE AccessKey=@Key AND Username=@Username AND ExpiresAt <= GetDATE()";
            string[] authParams = apiAuthKey.Split(':');

            using (SqlConnection _conn = new SqlConnection(Global.DefaultConnectionString))
            {
                _conn.Open();
                SqlCommand authCMD = new SqlCommand(query, _conn);
                authCMD.Parameters.Add("@Key", SqlDbType.Char, 64);
                authCMD.Parameters["@Key"].Value = authParams[0];
                authCMD.Parameters.Add("@Username", SqlDbType.VarChar, 20);
                authCMD.Parameters["@Username"].Value = authParams[1];

                using (SqlDataReader authReader = authCMD.ExecuteReader())
                {
                    return authReader.HasRows;
                }
            }
        }

        public static bool CheckAuthorisation(string apiAuthKey, string buisnessArea, string accessCode)
        {
            string groupsQuery = "SELECT sg.GroupName, sg.isAdmin, sg.isAudit" +
                "FROM dbo.SecurityGroupMembership AS SGM" +
                "    LEFT JOIN dbo.SecurityGroup AS SG ON SGM.GroupName = SG.GroupName" +
                "WHERE sgm.Username=@UN";

            string accessQuery = "SELECT ace.FC, ace.RM, ace.RO, ace.A, ace.D" +
                "FROM dbo.DocumentACE AS ace" +
                "WHERE ace.GroupName IN ({0}) AND ace.BuisnessArea=@BA";

            string[] authParams = apiAuthKey.Split(':');

            using (SqlConnection _conn = new SqlConnection(Global.DefaultConnectionString))
            {
                _conn.Open();
                SqlCommand groupsCMD = new SqlCommand(groupsQuery, _conn);
                groupsCMD.Parameters.Add("@UN", SqlDbType.VarChar, 20);
                groupsCMD.Parameters["@UN"].Value = authParams[1];

                List<string> groupNames = new List<string>();

                using (var groupsReader = groupsCMD.ExecuteReader())
                {
                    while (groupsReader.Read())
                    {
                        if (ReaderExtension.GetBool(groupsReader, "isAdmin"))
                        {
                            return true; //User is admin they have can do everything.
                        }
                        else if (ReaderExtension.GetBool(groupsReader, "isAudit") && (accessCode == "read"|| accessCode == "audit"))
                        {
                            return true; //User is an Auditor and can read everything.
                        }
                        else
                        {
                            groupNames.Add(ReaderExtension.GetString(groupsReader, "GroupName"));
                        }
                    }
                }

                if (groupNames.Count == 0)
                {
                    //Need an Access Group to get Access
                    return false;
                }

                //https://stackoverflow.com/questions/20739578/pass-string-array-as-parameter-in-sql-query-in-c-sharp
                //Add Dynamic Params for each member of groupNames
                var groupParams = String.Join(',', groupNames.Select((r, i) => "@gn" + i));

                accessQuery = String.Format(accessQuery, groupParams);

                SqlCommand accessCMD = new SqlCommand(accessQuery, _conn);
                accessCMD.Parameters.AddWithValue("@BA", buisnessArea);

                //Add all group names as parameters
                int ic = 0;
                foreach (var gn in groupNames)
                {
                    accessCMD.Parameters.AddWithValue("@gn" + ic, gn);
                    ic++;
                }

                using (var accessReader = accessCMD.ExecuteReader())
                {
                    while (accessReader.Read())
                    {
                        if (ReaderExtension.GetBool(accessReader, "FC")) //Full Control (we don't have to check the accessCode
                        {
                            return true;
                        }
                        else
                        {
                            //for all other access types check the accessCode
                            bool RM = ReaderExtension.GetBool(accessReader, "RM");
                            bool RO = ReaderExtension.GetBool(accessReader, "RO");
                            bool Ar = ReaderExtension.GetBool(accessReader, "A");
                            bool De = ReaderExtension.GetBool(accessReader, "D");
                            if (accessCode == "read" && (RM || RO))
                            {
                                return true;
                            }
                            else if (accessCode == "modify" && RM)
                            {
                                return true;
                            }
                            else if (accessCode == "archive" && Ar)
                            {
                                return true;
                            }
                            else if (accessCode == "delete" && De)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //public static Metadata GetDocumentMetadata(int id)
        //{
        //    Metadata result = new Metadata
        //    {
        //        CustomMetadata = new List<CustomMetadataItem>()
        //    };
        //    string cMetaQuery = "SELECT DocumentMetadataId AS MetadataId, [Key], " +
        //            "ValueStr, ValueInt, ValueDate, ValueBool " +
        //            "FROM dbo.CustomMetadata WHERE DocumentMetadataId=@ID;";

        //    string metaQuery = "SELECT TOP 1 dm.Id, dm.Name, dm.Version, dm.MD5Key, dm.StorName, " +
        //            "dm.Extension, dm.BuisnessArea, dm.isLocked, dm.LockedBy, dm.LockedAt, dm.LockExpires, " +
        //            "dm.isArchived, dm.ArchivedBy, dm.ArchivedAt, dm.CreatedBy, " +
        //            "dm.CreatedAt, dm.LastUpdateBy, dm.LastUpdateAt " +
        //            "FROM dbo.DocumentMetadata as dm WHERE dm.ID=@ID;";

        //    using (SqlConnection _conn = new SqlConnection(Global.DefaultConnectionString))
        //    {
        //        _conn.Open();

        //        SqlCommand metaCmd = new SqlCommand(metaQuery, _conn);
        //        metaCmd.Parameters.Add("@ID", SqlDbType.Int);
        //        metaCmd.Parameters["@ID"].Value = id;

        //        using (SqlDataReader metaReader = metaCmd.ExecuteReader())
        //        {
        //            while (metaReader.Read())
        //            {
        //                result.Id = ReaderExtension.GetInt(metaReader, "Id");
        //                result.Name = ReaderExtension.GetString(metaReader, "Name");
        //                result.Version = ReaderExtension.GetInt(metaReader, "Version");
        //                result.MD5Hash = ReaderExtension.GetString(metaReader, "MD5Key");
        //                result.StorName = ReaderExtension.GetString(metaReader, "StorName");
        //                result.Extension = ReaderExtension.GetString(metaReader, "Extension");
        //                result.BuisnessArea = ReaderExtension.GetString(metaReader, "BuisnessArea");
        //                result.IsLocked = ReaderExtension.GetBool(metaReader, "isLocked");
        //                result.LockedBy = ReaderExtension.GetString(metaReader, "LockedBy");
        //                result.LockedAt = ReaderExtension.GetNullableDate(metaReader, "LockedAt");
        //                result.LockExpiration = ReaderExtension.GetNullableDate(metaReader, "LockExpires");
        //                result.isArchived = ReaderExtension.GetBool(metaReader, "isArchived");
        //                result.ArchivedBy = ReaderExtension.GetString(metaReader, "ArchivedBy");
        //                result.ArchivedAt = ReaderExtension.GetNullableDate(metaReader, "ArchivedAt");
        //                result.CreatedBy = ReaderExtension.GetString(metaReader, "CreatedBy");
        //                result.CreatedAt = ReaderExtension.GetDate(metaReader, "CreatedAt");
        //                result.LastUpdateBy = ReaderExtension.GetString(metaReader, "LastUpdateBy");
        //                result.LastUpdateAt = ReaderExtension.GetDate(metaReader, "LastUpdateAt");
        //            }
        //        }

        //        SqlCommand cMetaCMD = new SqlCommand(cMetaQuery, _conn);
        //        cMetaCMD.Parameters.Add("@ID", SqlDbType.Int);
        //        cMetaCMD.Parameters["@ID"].Value = id;

        //        using (SqlDataReader cMetaRdr = cMetaCMD.ExecuteReader())
        //        {
        //            while (cMetaRdr.Read())
        //            {
        //                result.CustomMetadata.Add(ReadCMRow(cMetaRdr));
        //            }
        //        }
        //    }

        //    return result;
        //}

        //private static CustomMetadataItem ReadCMRow(SqlDataReader reader)
        //{
        //    CustomMetadataItem item = new CustomMetadataItem();
        //    item.Key = ReaderExtension.GetString(reader, "Key");

        //    if (ReaderExtension.IsNotNull(reader, "ValueStr"))
        //    {
        //        item.Value = ReaderExtension.GetString(reader, "ValueStr");
        //        item.ValueType = typeof(string);
        //        return item;
        //    }
        //    else if (ReaderExtension.IsNotNull(reader, "ValueInt"))
        //    {
        //        item.Value = ReaderExtension.GetInt(reader, "ValueInt");
        //        item.ValueType = typeof(int);
        //        return item;
        //    }
        //    else if (ReaderExtension.IsNotNull(reader, "ValueDate"))
        //    {
        //        item.Value = ReaderExtension.GetDate(reader, "ValueDate");
        //        item.ValueType = typeof(DateTime);
        //        return item;
        //    }
        //    else if (ReaderExtension.IsNotNull(reader, "ValueBool"))
        //    {
        //        item.Value = ReaderExtension.GetBool(reader, "ValueBool");
        //        item.ValueType = typeof(bool);
        //        return item;
        //    }
        //    else
        //    {
        //        int refID = ReaderExtension.GetInt(reader, "MetadataId");
        //        throw new Exception(String.Format("unable to read CustomMetadata Row, Metadata ID:'{0}' Key:'{1}'", refID, item.Key));
        //    }
            
        //}


        //public Object LockDocument(int id)
        //{
        //    return new object();
        //}

        //public void GetDocumentDownloadDetailsByID(int id, SqlConnection _conn)
        //{
        //    string baseQuery = "SELECT dm.Id, dm.Name, dm.Version, dm.MD5Key, dm.Extension, dm.BuisnessArea, dm.LockedBy, " +
        //        "dm.LockedAt, dm.LockExpires, dm.isArchived, dm.StorName, sc.StorType, sc.Args as StorArgs, sc.ENV As StorENV" +
        //        "FROM dbo.DocumentMetadata as dm" +
        //        "LEFT JOIN dbo.StorConfigs as sc ON dm.StorName = sc.ShortName" +
        //        "WHERE dm.ID = @ID";
        //}
    }
}
