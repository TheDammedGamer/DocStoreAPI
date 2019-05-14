using DocStoreAPI.Models;
using DocStoreAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DocStoreAPI.Shared
{
    public static class MetadataExtensions
    {
        public static MetadataEntity CompareMetadataEntity(this MetadataEntity entityOriginal, MetadataEntity entityNew, string userName, DocumentRepository documentRepository)
        {
            bool rename = false;
            bool changed = false;
            MetadataEntity result = new MetadataEntity();
            foreach (PropertyInfo property in entityOriginal.GetType().GetProperties())
            {
                var origValue = property.GetValue(entityOriginal);
                var newValue = property.GetValue(entityNew);

                //If Properties
                if (!origValue.Equals(newValue))
                {
                    changed = true;
                    switch (property.Name)
                    {
                        case "Id":
                            result.SetID(entityOriginal.Id);
                            break;
                        case "MD5Hash":
                            result.MD5Hash = entityOriginal.MD5Hash;
                            break;
                        case "Archive":
                            result.Archive = entityOriginal.Archive;
                            break;
                        case "Locked":
                            result.Locked = entityOriginal.Locked;
                            break;
                        case "Created":
                            result.Created = entityOriginal.Created;
                            break;
                        case "Versions":
                            result.Version = entityOriginal.Version;
                            break;
                        case "StorName":
                            result.StorName = entityOriginal.StorName;
                            break;
                        case "Name":
                            rename = true;
                            result.Name = entityNew.Name;
                            break;
                        case "Version":
                            rename = true;
                            result.Version = entityNew.Version;
                            break;
                        case "Extension":
                            rename = true;
                            result.Extension = entityNew.Extension;
                            break;
                        case "BuisnessAreaEntity":
                            result.BuisnessAreaEntity = entityNew.BuisnessAreaEntity;
                            break;
                        case "BuisnessArea":
                            result.BuisnessArea = entityNew.BuisnessAreaEntity.Name;
                            break;
                        case "CustomMetadata":
                            result.CustomMetadata = entityNew.CustomMetadata;
                            break;
                        case "LastUpdate":
                            result.LastUpdate = entityOriginal.LastUpdate;
                            break;
                        case "LastViewed":
                            result.LastViewed = entityOriginal.LastViewed;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    property.SetValue(result, origValue);
                }
            }

            if (changed)
            {
                result.LastUpdate.Update(userName);
                result.LastViewed = DateTime.UtcNow;
            }

            if (rename)
            {
                documentRepository.RenameFileAsync(entityOriginal, result).RunSynchronously();
            }

            return result;
        }

        
    }
}
