//=============================================================================
// <summary>
// SoundManager 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using blackfilter;
using via;
using via.attribute;
using via.audiorender;

namespace blackfilter
{
    [UpdateOrder((int)UpdateOrder.SoundManager)]
    public class SoundManager : Singleton<SoundManager>
    {
        #region Property

        [IgnoreDataMember, Browsable(false)]
        public static vec3 Position
        {
            get
            {
                var p = vec3.Zero;
                lock (_Lock)
                {
                    p = _Position;
                }
                return p;
            }
            set
            {
                lock (_Lock)
                {
                    _Position = value;
                }
            }
        }

        [IgnoreDataMember, Browsable(false)]
        public static Quaternion Rotation
        {
            set
            {
                _Rotation = value;
            }
            get
            {
                return _Rotation;
            }
        }


        #endregion  // Property

        #region Field

        [IgnoreDataMember, GroupSeparator, DisplayName("ボリューム")]
        static public bool _GroupSeparator_Volume = false;

        [Slider(MaxValue = 1, MinValue = 0)]
        [DataMember, DisplayName("マスターボリューム")]
        float _MasterVolume = 1.0f;

        [Slider(MaxValue = 1, MinValue = 0)]
        [DataMember, DisplayName("BGMボリューム")]
        float _BGMVolume = 1.0f;

        [Slider(MaxValue = 1, MinValue = 0)]
        [DataMember, DisplayName("SEボリューム")]
        float _SEVolume = 1.0f;

        [IgnoreDataMember, GroupEndSeparator]
        static public bool _GroupEndSeparator_Volume = false;

        [IgnoreDataMember, GroupSeparator, DisplayName("リスナー設定")]
        static public bool _GroupSeparator_Listener = false;

        [IgnoreDataMember, DisplayName("Y軸反転")]
        public bool RotateAxisYPiRad
        {
            get => _RotateAxisYPiRad;
            set => _RotateAxisYPiRad = value;
        }

        [DataMember, Browsable(false)]
        private bool _RotateAxisYPiRad = true;

        private static object _Lock = new object();

        [IgnoreDataMember, ReadOnly(true)]
        private static vec3 _Position = vec3.Zero;

        [IgnoreDataMember, ReadOnly(true)]
        private static Quaternion _Rotation = Quaternion.Identity;

        [IgnoreDataMember, GroupEndSeparator]
        static public bool _GroupEndSeparator_Listener = false;

        /// <summary>
        /// 共通SE用のSoundController
        /// </summary>
        private SoundController _SoundController = null;

        #endregion  // Field

        public void updateMasterVolume(float volume)
        {
            _MasterVolume = volume;
        }

        public void updateBgmVolume(float volume)
        {
            _BGMVolume = volume;
        }

        public void updateSeVolume(float volume)
        {
            _SEVolume = volume;
        }

        #region Method

        #region Base

        /// <summary>
        /// start
        /// </summary>
        public override void start()
        {
            set();

            // 共通SE用のSoundControllerを生成
            _SoundController = GameObject.getSameComponent<SoundController>();
        }

        public override void lateUpdate()
        {
            Camera mainCamera = SceneManager.MainView.PrimaryCamera;
            if (mainCamera != null)
            {
                Transform t = mainCamera.GameObject.Transform;
                _Position = t.Position;
                _Rotation = t.Rotation;
            }
            set();
        }

        public override void editUpdate()
        {
            set();
        }

        #endregion  // Base

        /// <summary>
        /// 位置と向きを設定
        /// </summary>
        private void set()
        {
            // 方向定位
            var rotation = _Rotation;
            if (_RotateAxisYPiRad)
            {
                rotation *= quaternion.makeRotationAxis(vec3.AxisY, math.PI);
            }
            //SendRequest.setListenerPositionRotation(0, _Position, _Rotation);
        }

        /// <summary>
        /// SEボリューム
        /// </summary>
        public float SeVolumeRate
        {
            get
            {
                return _SEVolume * _MasterVolume;
            }
        }

        /// <summary>
        /// BGMボリューム
        /// </summary>
        public float BGMVolumeRate
        {
            get
            {
                return _BGMVolume * _MasterVolume;
            }
        }

        /// <summary>
        /// Masterボリューム
        /// </summary>
        public float MasterVolumeRate
        {
            get
            {
                return _MasterVolume;
            }
        }

        /// <summary>
        /// 共通サウンドを再生
        /// </summary>
        /// <param name="id"></param>
        public void playCommonSound(int id)
        {
            _SoundController.play(id);
        }

        /// <summary>
        /// 共通サウンドを停止
        /// </summary>
        /// <param name="id"></param>
        public void stopCommonSound(int id)
        {
            _SoundController.stop(id);
        }

        /// <summary>
        /// フェードアウトして停止
        /// </summary>
        /// <param name="id"></param>
        public void fadeOutCommonSound(int id)
        {
            _SoundController.fadeOutAndStop(id);
        }

        #endregion  // Method
    }

}

