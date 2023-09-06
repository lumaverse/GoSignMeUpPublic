using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data.ViewModels.Grid;

namespace Gsmu.Api.Data.ViewModels.Layout
{
    public class LayoutConfiguration
    {
        public LayoutConfiguration()
        {
            SearchColumns = new CourseSearchViewConfiguration();
            LayoutButtons = new LayoutButtonConfiguration();
            IncreaseWordTopRow  = 13;
            HideStudentLogin = 0;
        }

        public LayoutButtonConfiguration LayoutButtons { get; set; }

        public CourseSearchViewConfiguration SearchColumns { get; set; }

        public int IncreaseWordTopRow
        {
            get;
            set;
        }

        public int HideStudentLogin
        {
            get;
            set;
        }

    }
}
