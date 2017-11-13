using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

namespace PartnerModeGo
{
    /// <summary>
    ///通过COM方式进行文本语音播放
    /// </summary>
    public class SpeechComLib
    {
        private static SpeechComLib instance = null;
        private static readonly Object padlock = new Object();
       

        object _spVoiceCls = null;//保存朗读用的 SAPI.SpVoice 
        const int SpFlags = 1; //SpeechVoiceSpeakFlags.SVSFlagsAsyn
        object _oISpeechObjectTokens = null; //保存 SAPI.ISpeechObjectTokens 就是系统有的语音引擎集合
        int TokensCount = 0;
        DictionaryEntry[] _deTokens = null;

        internal object SpVoiceCls { get { return _spVoiceCls; } }

        public static SpeechComLib Instance
        {
            get
            {
                if (instance==null)
                {
                    lock (SpeechComLib.padlock)
                    {
                        if (SpeechComLib.instance == null)
                        {
                            SpeechComLib.instance = new SpeechComLib();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SpeechComLib()
        {

            InitSAPI();

        }

        private void InitSAPI()
        {
            try
            {
                //创建语音对象朗读用
                _spVoiceCls = UserComHelper.CreateComObject("SAPI.SpVoice");
                if (_spVoiceCls == null)
                {
                    throw new Exception(String.Format("请检查系统是否安装了语音播报环境！"));
                }
                else
                {
                    //取得SAPI.ISpeechObjectTokens
                    _oISpeechObjectTokens = UserComHelper.CallComMethod("GetVoices", _spVoiceCls);
                    object r = UserComHelper.GetComPropery("Count", _oISpeechObjectTokens);
                    if (r is int)
                    {
                        TokensCount = (int)r;
                        if (TokensCount > 0)
                        {
                            //取得全部语音识别对象模块，及名称，以被以后使用
                            _deTokens = new DictionaryEntry[TokensCount];
                            for (int i = 0; i < TokensCount; i++)
                            {
                                //从集合中取出单个 识别对象模块
                                //返回 SAPI.SpObjectToken
                                object oSpObjectToken = UserComHelper.CallComMethod("Item", _oISpeechObjectTokens, i);
                                //取名称
                                string Description = UserComHelper.CallComMethod("GetDescription", oSpObjectToken) as string;
                                //放到 DictionaryEntry 对象中，key 是 识别对象模块，value 是名称
                                _deTokens[i] = new DictionaryEntry(oSpObjectToken, Description);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 进行语音播放
        /// </summary>
        /// <param name="VoiceText"></param>
        public void PlaySpeech(string VoiceText)
        {
              string msg = VoiceText;           
              if (_spVoiceCls != null && _deTokens!=null)
              {
                    try
                    { 
                        Nullable<DictionaryEntry> engineEntry=null ;
                        foreach (DictionaryEntry dEntry in _deTokens)
                        {
                            if (dEntry.Value.ToString().IndexOf("Huihui") >= 0)
                                engineEntry = dEntry;
                        }

                        if (engineEntry!=null)
                        { 
                            //设置语言引擎            
                            UserComHelper.SetComProperty("Voice", _spVoiceCls, ((DictionaryEntry)engineEntry).Key);
                            //调用Speak 函数，msg 是要播放的文本，1 是异步播放,因为是异步的 com 对象不立刻释放
                            UserComHelper.CallComMethod("Speak", _spVoiceCls, msg, SpFlags);
                        }
                    }
                    catch
                    {

                    }


                }

            
        }
        /// <summary>
        /// 释放语音播放对象
        /// </summary>
        public void ReleaseSpeaker()
        {
            if (instance!=null)
            {
                lock (SpeechComLib.padlock)
                {                    
                    if (_spVoiceCls != null)
                        Marshal.ReleaseComObject(_spVoiceCls);
                    instance = null;
 
                }
            }
        }
    }    
        internal class UserComHelper
        {
            /// <summary>
            /// 设置属性
            /// </summary>
            /// <param name="name">名称</param>
            /// <param name="value">值</param>
            public static void SetComProperty(string name, object o, object value)
            {
                Type t = o.GetType();
                t.InvokeMember(name, BindingFlags.Instance | BindingFlags.SetProperty, null, o, new object[] { value });
            }
            /// <summary>
            /// 获取属性
            /// </summary>
            /// <param name="name">名称</param>
            /// <param name="o"></param>
            /// <returns></returns>
            public static object GetComPropery(string name, object o)
            {
                Type t = o.GetType();
                return t.InvokeMember(name, BindingFlags.Instance | BindingFlags.GetProperty, null, o, null);
            }

            /// <summary>
            /// 调用方法
            /// </summary>
            /// <param name="name"></param>
            /// <param name="o"></param>
            /// <param name="parms"></param>
            /// <returns></returns>
            public static object CallComMethod(string name, object o, params object[] parms)
            {
                Type t = o.GetType();

                return t.InvokeMember(name, BindingFlags.Instance | BindingFlags.InvokeMethod, null, o, parms);
            }

            /// <summary>
            ///创建com对象
            /// </summary>
            /// <param name="FromProgID"></param>
            /// <returns></returns>
            public static object CreateComObject(string FromProgID)
            {
                Type comType = Type.GetTypeFromProgID(FromProgID);
                object rVar = null;
                if (comType != null)
                    rVar = System.Activator.CreateInstance(comType);

                return rVar;
            }
        }

}
