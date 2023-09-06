using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace Gsmu.Api.Data.School.User
{

    public class UploadImage
    {

        public static readonly string ItemUploadFolderPath = "~/Images/Profiles/";

    public static string UploadTempImage(HttpPostedFileBase file, string abv, int userid)
    {

        var Context = new SchoolEntities();
        string OldTempProfileImage = "";
        Entities.Student st = new Entities.Student();
        Entities.Instructor it = new Entities.Instructor();

        if (abv == "ST")
        {
            st = Context.Students.First(s => s.STUDENTID == userid);
            OldTempProfileImage = st.TempProfileImage;
        }
        else if (abv == "IT")
        {
            it = Context.Instructors.First(i => i.INSTRUCTORID == userid);
            OldTempProfileImage = it.TempProfileImage;
        }

        if (System.IO.File.Exists(HttpContext.Current.Request.MapPath(ItemUploadFolderPath + OldTempProfileImage))){
            System.IO.File.Delete(HttpContext.Current.Request.MapPath(ItemUploadFolderPath + OldTempProfileImage));}
       
        string extension = Path.GetExtension(file.FileName);
        string tempFileName = abv + userid.ToString() +"-"+ DateTime.Now.ToString("yyyyMMddTHHmmss") + extension;

        bool uploadstat = uploadFile(file, tempFileName);

        if (uploadstat)
        {

            if (abv == "ST")
            {
                st.TempProfileImage = tempFileName;
            }
            else if (abv == "IT")
            {
                it.TempProfileImage = tempFileName;
            }

            Context.SaveChanges();
            return tempFileName;
        }
        else
        {
            return "false";
        }

    }

    public static bool renameUploadFile(HttpPostedFileBase file, Int32 counter = 0)
    {
        var fileName = Path.GetFileName(file.FileName);

        string append = "item_";
        string finalFileName = append + ((counter).ToString()) + "_" + fileName;
        if (System.IO.File.Exists(HttpContext.Current.Request.MapPath(ItemUploadFolderPath + finalFileName)))
        {
            //file exists 
            return renameUploadFile(file, ++counter);
        }

        return uploadFile(file, finalFileName);
    }

    private static bool uploadFile(HttpPostedFileBase file, string fileName)
    {
        var path = Path.Combine(HttpContext.Current.Request.MapPath(ItemUploadFolderPath), fileName);
        string extension = Path.GetExtension(file.FileName);

        //make sure the file is valid
        if (!validateExtension(extension))
        {
            return false;
        }

        try
        {
            file.SaveAs(path);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool validateExtension(string extension)
    {
        extension = extension.ToLower();
        switch (extension)
        {
            case ".jpg":
                return true;
            case ".png":
                return true;
            case ".gif":
                return true;
            case ".jpeg":
                return true;
            default:
                return false;
        }
    }


    }
}
