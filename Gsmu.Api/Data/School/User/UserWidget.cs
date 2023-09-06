using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gsmu.Api.Data.School.User
{
    public class UserWidget
    {

        public int WidgetCount { get; set; }

        public int ColumnCount { get; set; }

        public List<WidgetColumn> Column { get; set; }

        public List<WidgetInfo> Widgets { get; set; }

        public List<WidgetItemList> WidgetItems { get; set; }
    }


    public class WidgetColumn
    {
        public int ID { get; set; }

        public int DispSort { get; set; }

        public int ColFlex { get; set; }

        public int WidthPer { get; set; }
    }

    public class WidgetInfo
    {
        public int ID { get; set; }

        public int ColID { get; set; }

        public int DispSort { get; set; }

        public string Name { get; set; }

        public int HeaderHT { get; set; }

        public string Title { get; set; }

        public string WidgetType { get; set; }

        public bool WithProfileImage { get; set; }


        public string Url { get; set; }

        public string PanelID { get; set; }
    }


    public class WidgetItemList
    {
        public int ID { get; set; }

        public int WidgetID { get; set; }

        public int DispSort { get; set; }

        public string FieldName { get; set; }

    }

    public class WidgetFldProp
    {

        public List<UserRegFieldSpecs> data { get; set; }

    }


}
