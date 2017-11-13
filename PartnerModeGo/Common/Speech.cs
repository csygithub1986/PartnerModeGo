using System;
using System.IO;
using System.Media;

namespace PartnerModeGo
{
    public class SoundSpeaker
    {
        private static SoundSpeaker instance = new Lazy<SoundSpeaker>(() => new SoundSpeaker(), true).Value;
        /// <summary>
        /// 获取一个值，该值指示此类型的唯一实例，此属性为只读。
        /// </summary>
        public static SoundSpeaker Instance
        {
            get
            {
                return SoundSpeaker.instance;
            }
        }

        private SpeechLib.SpVoice SpVoice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SoundSpeaker()
        {
            Type comType = Type.GetTypeFromProgID("SAPI.SpVoice");
            object rVar = null;
            if (comType != null)
            {
                rVar = System.Activator.CreateInstance(comType);
                SpVoice = rVar as SpeechLib.SpVoice;
            }
            /*设定语音
            if (SpVoice!=null)
            {
            var voices = SpVoice.GetVoices();
            var voice = voices.Item(0);
            var name = voice.GetDescription();
            var to = voice as SpeechLib.SpObjectToken;
            SpVoice.Voice = to;
            }
            //*/
        }

        private const int SpFlags = 1; //SpeechVoiceSpeakFlags.SVSFlagsAsyn

        /// <summary>
        /// 使用中文语音播放指定文字。
        /// </summary>
        /// <param name="text">要播放的文字</param>
        public void Speak(String text)
        {
            try
            {
                if (SpeechComLib.Instance.SpVoiceCls != null)
                {
                    //设置语言引擎-为兼容win7与win10不设置Voice引擎，系统采用默认引擎
                    //SpeechSoundHelper.SetComProperty("Voice", _spVoiceCls, comboBox1.SelectedValue);
                    //调用Speak 函数，msg 是要播放的文本，1 是异步播放,因为是异步的 com 对象不立刻释放
                    UserComHelper.CallComMethod("Speak", SpeechComLib.Instance.SpVoiceCls, text, SpFlags);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            //try
            //{
            //    if (String.IsNullOrEmpty(text))
            //    {
            //        return;
            //    }
            //    SpVoice?.Speak(text, SpeechLib.SpeechVoiceSpeakFlags.SVSFlagsAsync);
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show("EX:" + ex.ToString());
            //}
        }
    }
    /*
    /// <summary>
    /// 语音转换2
    /// </summary>
    public class SoundSpeaker
    {
        #region 静态字段
        private static SoundSpeaker instance = null;
        private static readonly Object padlock = new Object();
        #endregion

        #region 字段
        private SpeechSynthesizer m_synthesizer;
        private SoundPlayer m_player;
        #endregion

        #region 静态属性
        /// <summary>
        /// 获取一个值，该值指示此类型的唯一实例，此属性为只读。
        /// </summary>
        public static SoundSpeaker Instance
        {
            get
            {
                if (SoundSpeaker.instance == null)
                {
                    lock (SoundSpeaker.padlock)
                    {
                        if (SoundSpeaker.instance == null)
                        {
                            SoundSpeaker.instance = new SoundSpeaker();
                        }
                    }
                }
                return SoundSpeaker.instance;
            }
        }
        #endregion

        #region 构造函数
        private SoundSpeaker()
        {
            try
            {
                this.m_synthesizer = new SpeechSynthesizer();
                this.m_synthesizer.Rate = 0;
                this.m_synthesizer.Volume = 100;
                this.m_synthesizer.SelectVoice("Microsoft Huihui Desktop - Chinese (Simplified)");
                this.m_synthesizer.SpeakCompleted += m_synthesizer_SpeakCompleted;
                this.m_player = new SoundPlayer();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region 事件函数
        private void m_synthesizer_SpeakCompleted(Object sender, SpeakCompletedEventArgs e)
        {
            try
            {
                m_player.Stream.Position = 0;
                m_player.Play();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region 公共函数
        /// <summary>
        /// 使用中文语音播放指定文字。
        /// </summary>
        /// <param name="text">要播放的文字</param>
        public void Speak(String text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            try
            {
                this.m_player.Stream = new MemoryStream();
                this.m_synthesizer.SetOutputToWaveStream(this.m_player.Stream);
                this.m_synthesizer.SpeakAsync(text);
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
    //*/
}