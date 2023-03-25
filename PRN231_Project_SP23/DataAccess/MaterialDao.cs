﻿using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MaterialDao
    {
        public static IEnumerable<Material> GetMaterialsByCourseId(int courseId)
        {
            List<Material> list = new List<Material>();
            try
            {
                using (var context = new PRN231_ProjectContext())
                {
                    list = context.Materials.Include(m => m.Course).Include(m => m.Uploader).Where(a => a.CourseId == courseId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public static void SaveMaterial(IFormFile material, string materialPath, int courseId, int uploaderId, string materialName)
        {
            using (var context = new PRN231_ProjectContext())
            {
                using(var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Material newMaterial = new Material
                        {
                            CourseId = courseId,
                            MaterialName = materialName,
                            Path = materialPath,
                            UploaderId = uploaderId
                        };
                        context.Materials.Add(newMaterial);
                        string materialpath2 = Path.Combine(materialPath, materialName);
                        if (context.SaveChanges() > 0)
                        {
                            transaction.Commit();
                            if (System.IO.File.Exists(materialpath2))
                            {
                                System.IO.File.Delete(materialpath2);
                            }
                            material.CopyTo(new FileStream(materialpath2, FileMode.Create));
                        }
                    }
                    catch(Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
    }
}
