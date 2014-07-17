using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pkg_Checker.Helpers
{
    static class Configuration
    {
        // import the API in kernel32.dll
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        #region Configuration Fields
        public static String R_Load { get; set; }
        public static String R_Farea { get; set; }
        public static char R_DOLevel { get; set; }
        public static String ACM_Project { get; set; }
        public static String ACM_Subproject { get; set; }
        public static String R_ArtifactProduced { get; set; }
        public static String R_SupplierLocation { get; set; }
        public static String R_Aircraft { get; set; }
        public static float Min_M_Duration { get; set; }  // min meeting duration
        public static int Min_Review_Participants { get; set; }
        #endregion

        public static void ReadCfg()
        {            
            try
            {                
                R_Load = Properties.Settings.Default.Load;
                R_Farea = Properties.Settings.Default.FAREA;
                R_DOLevel = Properties.Settings.Default.DO178Level;
                ACM_Project = Properties.Settings.Default.ACM_Project;
                ACM_Subproject = Properties.Settings.Default.ACM_Subproject;
                R_ArtifactProduced = Properties.Settings.Default.Produced;
                R_SupplierLocation = Properties.Settings.Default.Producer_Employer;
                R_Aircraft = Properties.Settings.Default.Aircraft;
                Min_M_Duration = Properties.Settings.Default.Min_Meeting_Duration;
                Min_Review_Participants = Properties.Settings.Default.Min_Review_Participants;

                #region ini configuration

                /*
                GetPrivateProfileString(SectionCoverSheet, "Load", "", StrBuilder, 64, cfgPath);
                R_Load = StrBuilder.ToString();

                GetPrivateProfileString(SectionCoverSheet, "FAREA", "TESTS", StrBuilder, 64, cfgPath);
                R_Farea = StrBuilder.ToString();

                GetPrivateProfileString(SectionCoverSheet, "DO178Level", "B", StrBuilder, 64, cfgPath);
                R_DOLevel = StrBuilder.ToString();

                GetPrivateProfileString(SectionCoverSheet, "ACM_Project", "", StrBuilder, 64, cfgPath);
                ACM_Project = StrBuilder.ToString();

                GetPrivateProfileString(SectionCoverSheet, "ACM_Subproject", "", StrBuilder, 64, cfgPath);
                ACM_Subproject = StrBuilder.ToString();

                GetPrivateProfileString(SectionCoverSheet, "Produced", "", StrBuilder, 64, cfgPath);
                R_ArtifactProduced = StrBuilder.ToString();

                GetPrivateProfileString(SectionCoverSheet, "Producer_Employer", "", StrBuilder, 64, cfgPath);
                R_SupplierLocation = StrBuilder.ToString();

                GetPrivateProfileString(SectionCoverSheet, "Aircraft", "", StrBuilder, 64, cfgPath);
                R_Aircraft = StrBuilder.ToString();

                GetPrivateProfileString(SectionCoverSheet, "Min_Meeting_Duration", "0.5", StrBuilder, 64, cfgPath);
                Min_M_Duration = Convert.ToSingle(StrBuilder.ToString());

                GetPrivateProfileString(SectionCoverSheet, "Min_Review_Participants", "3", StrBuilder, 64, cfgPath);
                Min_Review_Participants = Convert.ToInt32(StrBuilder.ToString());
                 */

                #endregion // ini configuration
            }

            catch (Exception e)
            {
                MessageBox.Show("Error while reading the configuration:" + e.Message,
                    "Failure in reading configuration file", MessageBoxButtons.AbortRetryIgnore);
            }
        }
    }
}
