namespace VrGamesDev.BHEL
{
    /// <summary>
    /// This struct saves the logs events while the system is not fully loaded
    /// </summary>
    [System.Serializable]
    public struct VRG_LogsBuffer
    {
        /// <summary>
        /// The timestamp when the logs tried to save the information, you configure it in the <a href="index.html#VrGamesDev.BHEL.VRG_LogsBuffer.VRG_LogsBuffer(string,%20string,%20ENUM_Verbose)">Basic constructor</a>
        /// </summary>
        public string time;

        /// <summary>
        /// The value of the message, you configure it in the <a href="index.html#VrGamesDev.BHEL.VRG_LogsBuffer.VRG_LogsBuffer(string,%20string,%20ENUM_Verbose)">Basic constructor</a>
        /// </summary>
        public string value;

        /// <summary>
        /// The level of verbosing, you configure it in the <a href="index.html#VrGamesDev.BHEL.VRG_LogsBuffer.VRG_LogsBuffer(string,%20string,%20ENUM_Verbose)">Basic constructor</a>
        /// </summary>
        public ENUM_Verbose verbose;



        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="timeLocal"><a href="index.html#VrGamesDev.BHEL.VRG_LogsBuffer.time">Class property this.time</a></param>
        /// <param name="valueLocal"><a href="index.html#VrGamesDev.BHEL.VRG_LogsBuffer.value">Class property this.value</a></param>
        /// <param name="verboseLocal"><a href="index.html#VrGamesDev.BHEL.VRG_LogsBuffer.verbose">Class property this.verbose</a></param>
        public VRG_LogsBuffer(string timeLocal, string valueLocal, ENUM_Verbose verboseLocal)
        {
            this.time = timeLocal;
            this.value = valueLocal;
            this.verbose = verboseLocal;
        }
    }



    /// <summary>
    /// The append mode, there are 3 modes, Overwrite, Append, diferent files
    /// </summary>
    public enum ENUM_Append
    {
        /// <summary>
        /// ENUM_Append.OVERWRITE = Write mode, it replace all the content of the file and
        /// overwrite it with the new information
        /// </summary>
        OVERWRITE = 0,

        /// <summary>
        /// ENUM_Append.APPEND It saves the content of the file and write the new
        /// information at the bottom
        /// </summary>
        APPEND = 1,

        /// <summary>
        /// ENUM_Verbose.DIFFERENT_FILES Every single file is saved in a new file the name
        /// of the file is timestamped
        /// </summary>
        DIFFERENT_FILES = 2
    }



    /// #IGNORE
    public class VRG_DataBaseBHEL : VRG_Base { }
}