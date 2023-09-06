using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Integration.Haiku.Import
{
    public class HaikuImportCvs
    {
        public HaikuImportCvs(string[] result)
        {
            var userImportId = result[0];
            UserImportId = userImportId;

            var userId = result[1];
            UserId = HaikuCsvImport.ParseIntNull(userId);

            var classImportId = result[2];
            ClassImportId = classImportId;

            var classId = result[3];
            ClassID = HaikuCsvImport.ParseIntNull(classId);

            var termName = result[4];
            TermName = result[4];

            var gradingPeriodName = result[5];
            GradingPeriodName = gradingPeriodName;

            var scoreUpdatedAt = result[6];
            ScoreUpdatedAt = HaikuCsvImport.DateTimeParse(scoreUpdatedAt);
            
            var scoreDeletedAt = result[7];
            ScoreDeletedAt = HaikuCsvImport.DateTimeParse(scoreDeletedAt);

            var letterGrade = result[8];
            LetterGrade = letterGrade;

            var precentGrade = result[9];
            PercentGrade = HaikuCsvImport.ParseFloatNull(precentGrade);

        }

        public string UserImportId
        {
            get;
            set;
        }

        public int? UserId
        {
            get;
            set;
        }

        public string ClassImportId
        {
            get;
            set;
        }

        public int? ClassID
        {
            get;
            set;
        }

        public string TermName
        {
            get;
            set;
        }

        public string GradingPeriodName
        {
            get;
            set;
        }

        public DateTime? ScoreUpdatedAt
        {
            get;
            set;
        }

        public DateTime? ScoreDeletedAt
        {
            get;
            set;
        }

        public string LetterGrade
        {
            get;
            set;
        }

        public float? PercentGrade
        {
            get;
            set;
        }

    }
}
