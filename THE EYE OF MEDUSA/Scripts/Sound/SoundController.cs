//=============================================================================
// <summary>
// SoundController 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using System;
using System.Collections.Generic;
using System.Numerics;
using via;
using via.attribute;
using via.audiorender;
using via.landscape;
using via.sound;

namespace blackfilter
{
    public static class SoundConstants
    {
        public static uint InvalidId = 0xFFFFFFFF;
        public static float ZeroDecibel = 0.0f;
        public static uint SourceVolumeId = 9900;
        public static uint DistanceAttenuationVolumeId = 9901;
    }

    public enum PanningType
    {
        Direct,
        Position,
        Angle,
    }

    public enum SoundKind
    {
        BGM,
        SE,
    }

    [DynamicDisplayName(nameof(SourceName))]
    public class SoundSource
    {
        #region Property
        [DataMember, DisplayName("リクエストID"), DisplayOrder(0)]
        int _RequestId = -1;

        public int RequestId
        {
            get => _RequestId;
        }

        [DataMember, DisplayName("種類")]
        SoundKind _Kind = SoundKind.SE;

        // プロパティ表示グループ設定
        [IgnoreDataMember, GroupSeparator, DisplayName("波形設定"), DisplayOrder(100)]
        const bool SourceInfoGroup = true;
        [IgnoreDataMember, GroupSeparator, DisplayName("再生設定"), DisplayOrder(200)]
        const bool PlaySettingGroup = true;
        [IgnoreDataMember, GroupSeparator, DisplayName("3D再生設定"), DisplayOrder(300)]
        const bool SpatializeSettingGroup = true;
        [IgnoreDataMember, GroupSeparator, DisplayName("再生中の音の情報"), DisplayOrder(400)]
        const bool PlayingInfoGroup = true;
        [IgnoreDataMember, GroupEndSeparator]
        private const bool GroupEnd = true;

        // 再生設定変更フラグ
        [IgnoreDataMember, Browsable(false)]
        bool _IsParamChanged = false;

        #region 波形設定 SourceInfoGroup
        [IgnoreDataMember, DisplayOrder(nameof(SourceInfoGroup), 101)]
        [DisplayName("波形"), Description("再生対象の波形を設定します。")]
        public SourceResourceHolder SourceAsset
        {
            get => _SourceAsset;
            set
            {
                _SourceAsset = value;
                updateSourceInfo();
            }
        }
        [DataMember, Browsable(false)]
        SourceResourceHolder _SourceAsset;

        [IgnoreDataMember, DisplayOrder(nameof(SourceInfoGroup), 102), ReadOnly(true)]
        [DisplayName("チャンネル数"), Description("波形のチャンネル数です。")]
        public uint Channel;

        [IgnoreDataMember, DisplayOrder(nameof(SourceInfoGroup), 103), ReadOnly(true)]
        [DisplayName("波形の長さ(s)"), Description("波形の長さ(秒)です。")]
        public float DurationSec;

        [IgnoreDataMember, Browsable(false)]
        public string SourceName
        {
            get
            {
                var res_path = _SourceAsset.ResourcePath.Split('/');
                string res_str = string.Format("{0}", res_path[res_path.Length - 1]);
                res_str = res_str.Split('.')[0];
                return "[ID:" + _RequestId.ToString() + "][" + _Kind.ToString() + "]" + res_str;
            }
        }
        #endregion

        #region 再生設定 PlaySettingGroup
        [DisplayOrder(nameof(PlaySettingGroup), 201)]
        [DisplayName("再生中の音に反映"), Description("再生設定を変更したとき、再生中の音にもその変更を反映させるかどうかのフラグです。")]
        public bool ApplyToPlayingSources = true;

        [IgnoreDataMember, DisplayOrder(nameof(PlaySettingGroup), 210)]
        [DisplayName("音量"), Description("再生時の音量(0～1)です。")]
        [Slider(0.0f, 1.0f)]
        public float Volume
        {
            get
            {
                switch(_Kind)
                {
                    case SoundKind.BGM:
                        return SoundManager.Instance.BGMVolumeRate;
                    case SoundKind.SE:
                        return SoundManager.Instance.SeVolumeRate;
                    default:
                        return 0;
                }
            }
            set
            {
                _Volume = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        float _Volume = 1.0f;
        float _CurrentVolume = 1.0f;

        [IgnoreDataMember, DisplayOrder(nameof(PlaySettingGroup), 211)]
        [DisplayName("ピッチ"), Description("再生時の再生速度(0.2～4.0)です。")]
        [Slider(0.01f, 4.0f)]
        public float Pitch
        {
            get => _Pitch;
            set
            {
                _Pitch = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        float _Pitch = 1.0f;
        float _CurrentPitch = 1.0f;

        [IgnoreDataMember, DisplayOrder(nameof(PlaySettingGroup), 212)]
        [DisplayName("バスID"), Description("再生時の送り先バスIDです。")]
        [BusIdSelector]
        public uint BusId
        {
            get => _BusId;
            set
            {
                _BusId = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        uint _BusId = AudioRenderer.MasterAudioBusId;
        uint _CurrentBusId = via.audiorender.AudioRenderer.MasterAudioBusId;

        [DisplayOrder(nameof(PlaySettingGroup), 213)]
        [DisplayName("最大同時発音数"), Description("同時に発音できる最大数です。")]
        [NumberEdit(1, 64)]
        public uint MaxVoiceCount = 64;

        [IgnoreDataMember, Browsable(true), DisplayOrder(nameof(PlaySettingGroup), 214)]
        [DisplayName("ループ有効/無効"), Description("再生対象の波形のループ設定です。")]
        public bool Loop
        {
            get => _Loop;
            set
            {
                _Loop = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        bool _Loop = false;
        bool _CurrentLoop = false;

        [DisplayOrder(nameof(PlaySettingGroup), 220)]
        [DisplayName("フェードイン時間"), Description("再生開始時のフェードイン時間(s)です。")]
        [NumberEdit(MinValue = 0.0, StepValue = 0.1)]
        public float FadeInTimeSec;

        [DisplayOrder(nameof(PlaySettingGroup), 221)]
        [NumberEdit(MinValue = 0.0, StepValue = 0.1)]
        [DisplayName("フェードアウト時間"), Description("再生終了時のフェードアウト時間(s)です。波形を最後まで再生する場合とfadeOutAndStop()で停止される場合に適用されます。")]
        public float FadeOutTimeSec;
        #endregion

        #region 再生位置設定 SpatializeSettingGroup

        [IgnoreDataMember, Browsable(true), DisplayOrder(nameof(SpatializeSettingGroup), 310)]
        [DisplayName("パンニング設定"), Description("パンニング設定を行います。Directはそのまま再生、Positionは位置によるパンニング、Angleは角度によるパンニングを行います。モノラル波形のみ変更可能です。")]
        public PanningType PanningType
        {
            get => _PanningType;
            set
            {
                _PanningType = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        PanningType _PanningType;
        PanningType _CurrentPanningType;

        [IgnoreDataMember, Browsable(false)]
        bool IsPositionPanning => _PanningType == PanningType.Position;

        [IgnoreDataMember, Browsable(false)]
        bool IsAnglePanning => _PanningType == PanningType.Angle;

        [DynamicBrowsable(nameof(IsPositionPanning)), DisplayOrder(nameof(SpatializeSettingGroup), 311)]
        [DisplayName("位置追従有効/無効"), Description("Sourceの再生中、再生位置を追従するかどうかです。falseの場合、再生位置は再生開始以降更新されません。")]
        public bool EnableFollowPosition;

        [IgnoreDataMember, Browsable(false)]
        public vec3 Position;
        [IgnoreDataMember, DynamicBrowsable(nameof(IsPositionPanning)), DisplayOrder(nameof(SpatializeSettingGroup), 312), ReadOnly(true)]
        [DisplayName("現在の再生位置"), Description("現在のSourceの再生位置です。")]
        vec3 _CurrentPosition;

        [IgnoreDataMember, Browsable(true), DisplayOrder(nameof(SpatializeSettingGroup), 350)]
        [DisplayName("距離減衰有効/無効"), Description("距離による音量の減衰を有効にするかどうかです。")]
        public bool EnableDistanceAttenuation
        {
            get => _EnableDistanceAttenuation;
            set
            {
                _EnableDistanceAttenuation = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        bool _EnableDistanceAttenuation = false;
        bool _CurrentEnableDistanceAttenuation = false;

        [IgnoreDataMember, DynamicBrowsable(nameof(EnableDistanceAttenuation)), DisplayOrder(nameof(SpatializeSettingGroup), 351)]
        [DisplayName("距離減衰:最小距離"), Description("リスナとの距離がこれより小さい場合、最大音量となります。")]
        [NumberEdit(MinValue = 0.0, StepValue = 0.1)]
        public float MinDistanceAttenuation
        {
            get => _MinDistanceAttenuation;
            set
            {
                _MinDistanceAttenuation = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        float _MinDistanceAttenuation = 0.0f;
        float _CurrentMinDistanceAttenuation = 0.0f;

        [IgnoreDataMember, DynamicBrowsable(nameof(EnableDistanceAttenuation)), DisplayOrder(nameof(SpatializeSettingGroup), 352)]
        [DisplayName("距離減衰:最大距離"), Description("リスナとの距離がこれより大きい場合、音量が0となります。")]
        [NumberEdit(MinValue = 0.0, StepValue = 0.1)]
        public float MaxDistanceAttenuation
        {
            get => _MaxDistanceAttenuation;
            set
            {
                _MaxDistanceAttenuation = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        float _MaxDistanceAttenuation = 10.0f;
        float _CurrentMaxDistanceAttenuation = 10.0f;

        [IgnoreDataMember, DynamicBrowsable(nameof(IsAnglePanning)), DisplayOrder(nameof(SpatializeSettingGroup), 312)]
        [DisplayName("パンニング角度"), Description("音量が最小の距離です。")]
        [NumberEdit(StepValue = 0.1f)]
        public float Angle
        {
            get => _Angle;
            set
            {
                _Angle = value;
                _IsParamChanged = true;
            }
        }
        [DataMember, Browsable(false)]
        float _Angle = 0.0f;
        float _CurrentAngle = 0.0f;

        #endregion

        [IgnoreDataMember, DisplayOrder(nameof(PlayingInfoGroup)), ReadOnly(true)]
        [DisplayName("再生中のSourceのRequestID"), Description("再生中のSourceのRequestIDです。一時停止中のSourceも含まれます。")]
        public List<uint> PlayingSourceRequestIdList = new List<uint>();

        [IgnoreDataMember, DisplayOrder(nameof(PlayingInfoGroup)), ReadOnly(true)]
        [DisplayName("一時停止中のSourceのRequestID"), Description("一時停止中のSourceのRequestIDです。")]
        public List<uint> PausingSourceRequestIdList = new List<uint>();

        #endregion

        #region Method

        public void update()
        {
            // Source情報の更新の必要があれば更新する
            if (SourceAsset == null)
            {
                Channel = 0U;
                DurationSec = 0.0f;
            }
            else if (Channel == 0)
            {
                updateSourceInfo();
            }

            // 停止したSourceがあればそれをPlayingSourceRequestIDListに反映させる
            var stopRequestIdList = AudioRenderer.StopRequestId;
            foreach (var item in stopRequestIdList)
            {
                if (PlayingSourceRequestIdList.Contains(item))
                {
                    PlayingSourceRequestIdList.Remove(item);
                }
                if (PausingSourceRequestIdList.Contains(item))
                {
                    PausingSourceRequestIdList.Remove(item);
                }
            }

            // 最大発音数の反映 最大発音数を超えた数のSourceが再生されている場合、古いものから停止する
            if (PlayingSourceRequestIdList.Count > MaxVoiceCount)
            {
                var stopCount = PlayingSourceRequestIdList.Count - MaxVoiceCount;
                var stopIdList = new List<uint>();
                for (int i = 0; i < stopCount; i++)
                {
                    stopIdList.Add(PlayingSourceRequestIdList[i]);
                }
                foreach (var item in stopIdList)
                {
                    SendRequest.stop(item);
                    PlayingSourceRequestIdList.Remove(item);
                }
            }

            switch (_Kind)
            {
                case SoundKind.BGM:
                    if (_CurrentVolume != SoundManager.Instance.BGMVolumeRate)
                    {
                        _IsParamChanged = true;
                    }
                    break;
                case SoundKind.SE:
                    if (_CurrentVolume != SoundManager.Instance.SeVolumeRate)
                    {
                        _IsParamChanged = true;
                    }
                    break;
            }

            // 再生設定が変更されているとき、再生中のSourceがあればその再生設定を更新する
            if (ApplyToPlayingSources && PlayingSourceRequestIdList.Count > 0 && _IsParamChanged)
            {
                switch(_Kind)
                {
                    case SoundKind.BGM:
                        _Volume = SoundManager.Instance.BGMVolumeRate;
                        break;
                    case SoundKind.SE:
                        _Volume = SoundManager.Instance.SeVolumeRate;
                        break;
                }
                if (_CurrentVolume != _Volume)
                {
                    foreach (var item in PlayingSourceRequestIdList)
                    {
                        SendRequest.setInteractiveVolume(item, SoundConstants.SourceVolumeId, Util.mag2db(_Volume));
                    }
                    _CurrentVolume = _Volume;
                }
                if (_CurrentPitch != _Pitch)
                {
                    foreach (var item in PlayingSourceRequestIdList)
                    {
                        SendRequest.setPitchByRatio(item, _Pitch);
                    }
                    _CurrentPitch = _Pitch;
                }
                if (_CurrentBusId != _BusId)
                {
                    foreach (var item in PlayingSourceRequestIdList)
                    {
                        SendRequest.stopSendingBus(item, _CurrentBusId);
                        SendRequest.startSendingBus(item, _BusId);
                    }
                    _CurrentBusId = _BusId;
                }

                if (_CurrentLoop != _Loop)
                {
                    foreach (var item in PlayingSourceRequestIdList)
                    {
                        SendRequest.setLoop(item, _Loop);
                    }
                    _CurrentLoop = _Loop;
                }

                switch (_PanningType)
                {
                    case PanningType.Direct:
                        {
                            if (_CurrentPanningType != _PanningType)
                            {
                                foreach (var item in PlayingSourceRequestIdList)
                                {
                                    SendRequest.setPanEnable(item, false);
                                }
                            }
                            break;
                        }
                    case PanningType.Position:
                        {
                            if (_CurrentPanningType != _PanningType)
                            {
                                foreach (var item in PlayingSourceRequestIdList)
                                {
                                    SendRequest.setPanTypePosition(item);
                                    SendRequest.setSourcePosition(item, Position);
                                    _CurrentPosition = Position;
                                }
                            }
                            else
                            {
                                if (_CurrentPosition != Position && EnableFollowPosition)
                                {
                                    foreach (var item in PlayingSourceRequestIdList)
                                    {
                                        SendRequest.setSourcePosition(item, Position);
                                        _CurrentPosition = Position;
                                    }
                                }
                            }
                            break;
                        }
                    case PanningType.Angle:
                        {
                            if (_CurrentPanningType != _PanningType)
                            {
                                foreach (var item in PlayingSourceRequestIdList)
                                {
                                    SendRequest.setPanTypeAngle(item);
                                    SendRequest.setPan(item, _Angle);
                                    _CurrentAngle = _Angle;
                                }
                            }
                            else
                            {
                                if (_CurrentAngle != _Angle)
                                {
                                    foreach (var item in PlayingSourceRequestIdList)
                                    {
                                        SendRequest.setPan(item, _Angle);
                                        _CurrentAngle = _Angle;
                                    }
                                }
                            }
                            break;
                        }
                }

            }
            _IsParamChanged = false;

            // 距離減衰更新
            if (_EnableDistanceAttenuation)
            {
                var attenuationVolumeDb = 0.0f;
                var distance = vector.distance(Position, AudioRenderer.ListenerPosition);
                if (distance < MaxDistanceAttenuation && distance > MinDistanceAttenuation)
                {
                    var ratio = (distance - MinDistanceAttenuation) / (MaxDistanceAttenuation - MinDistanceAttenuation);
                    var volumeRatio = math.cosf(ratio * math.PIDIV2);
                    attenuationVolumeDb = Util.mag2db(volumeRatio);
                }
                else if (distance >= MaxDistanceAttenuation)
                {
                    attenuationVolumeDb = -100.0f;
                }
                foreach (var item in PlayingSourceRequestIdList)
                {
                    SendRequest.setInteractiveVolume(item, SoundConstants.DistanceAttenuationVolumeId, attenuationVolumeDb);
                }
                _CurrentEnableDistanceAttenuation = _EnableDistanceAttenuation;
            }
            else
            {
                if (_CurrentEnableDistanceAttenuation != _EnableDistanceAttenuation)
                {
                    foreach (var item in PlayingSourceRequestIdList)
                    {
                        SendRequest.setInteractiveVolume(item, SoundConstants.DistanceAttenuationVolumeId, 0.0f);
                    }
                    _CurrentEnableDistanceAttenuation = _EnableDistanceAttenuation;
                }
            }

            // 位置更新
            if (_CurrentPosition != Position && EnableFollowPosition)
            {
                foreach (var item in PlayingSourceRequestIdList)
                {
                    SendRequest.setSourcePosition(item, Position);
                    _CurrentPosition = Position;
                }
            }
        }

        [Action, DisplayName("Play"), Description("音を再生します。一時停止中であれば続きから再生します。")]
        public void play()
        {
            if (SourceAsset == null)
            {
                return;
            }

            if (PausingSourceRequestIdList.Count > 0)
            {
                // 一時停止中のSourceが存在するとき、それを再生する
                foreach (var item in PausingSourceRequestIdList)
                {
                    SendRequest.resume(item);
                }
                PausingSourceRequestIdList.Clear();
                return;
            }

            // 一時停止中のSourceが存在しないとき、新しくSourceを再生する
            if (PlayingSourceRequestIdList.Count >= MaxVoiceCount)
            {
                via.debug.infoLine($"{SourceName}は最大発音数に達しているため再生されません。");
                return;
            }

            var sourceId = SourceAccessor.getId(_SourceAsset);
            var requestId = SendRequest.prepare(sourceId);
            if (requestId == SoundConstants.InvalidId)
            {
                return;
            }
            PlayingSourceRequestIdList.Add(requestId);

            // 音量設定
            SendRequest.setInteractiveVolume(requestId, SoundConstants.SourceVolumeId, Util.mag2db(_Volume));
            if (!ApplyToPlayingSources || PlayingSourceRequestIdList.Count == 1)
            {
                _CurrentVolume = _Volume;
            }

            // ピッチ設定
            SendRequest.setPitchByRatio(requestId, _Pitch);
            if (!ApplyToPlayingSources || PlayingSourceRequestIdList.Count == 1)
            {
                _CurrentPitch = _Pitch;
            }

            // 送り先バス設定
            SendRequest.startSendingBus(requestId, _BusId);
            if (!ApplyToPlayingSources || PlayingSourceRequestIdList.Count == 1)
            {
                _CurrentBusId = _BusId;
            }

            // ループ設定
            SendRequest.setLoop(requestId, _Loop);
            if (!ApplyToPlayingSources || PlayingSourceRequestIdList.Count == 1)
            {
                _CurrentLoop = _Loop;
            }

            // フェードアウト設定
            if (FadeOutTimeSec > 0)
            {
                SendRequest.setPlayEndFadeOutTimeSec(requestId, FadeOutTimeSec);
            }

            switch (PanningType)
            {
                case PanningType.Position:
                    {
                        SendRequest.setPanTypePosition(requestId);
                        SendRequest.setSourcePosition(requestId, Position);
                        if (!ApplyToPlayingSources || PlayingSourceRequestIdList.Count == 1)
                        {
                            _CurrentPosition = Position;
                        }
                        break;
                    }
                case PanningType.Angle:
                    {
                        SendRequest.setPanTypeAngle(requestId);
                        SendRequest.setPan(requestId, _Angle);
                        if (!ApplyToPlayingSources || PlayingSourceRequestIdList.Count == 1)
                        {
                            _CurrentAngle = _Angle;
                        }
                        break;
                    }

            }

            // 距離減衰
            if (_EnableDistanceAttenuation)
            {
                var attenuationVolumeDb = 0.0f;
                var distance = vector.distance(Position, AudioRenderer.ListenerPosition);
                if (distance < MaxDistanceAttenuation && distance > MinDistanceAttenuation)
                {
                    var ratio = (distance - MinDistanceAttenuation) / (MaxDistanceAttenuation - MinDistanceAttenuation);
                    var volumeRatio = math.cosf(ratio * math.PIDIV2);
                    attenuationVolumeDb = Util.mag2db(volumeRatio);
                }
                else if (distance >= MaxDistanceAttenuation)
                {
                    attenuationVolumeDb = -100.0f;
                }
                foreach (var item in PlayingSourceRequestIdList)
                {
                    SendRequest.setInteractiveVolume(item, SoundConstants.DistanceAttenuationVolumeId, attenuationVolumeDb);
                }
                if (!ApplyToPlayingSources || PlayingSourceRequestIdList.Count == 1)
                {
                    _CurrentEnableDistanceAttenuation = _EnableDistanceAttenuation;
                }
            }

            // 再生処理
            if (FadeInTimeSec > 0.0f)
            {
                SendRequest.fadeInAndResume(requestId, FadeInTimeSec);
            }
            else
            {
                SendRequest.resume(requestId);
            }

        }

        [Action, DisplayName("Pause"), Description("再生中の音を一時停止します。")]
        public void pause()
        {
            foreach (var item in PlayingSourceRequestIdList)
            {
                SendRequest.pause(item);
                PausingSourceRequestIdList.Add(item);
            }
        }

        [Action, DisplayName("Stop"), Description("再生中の音を停止します。FadeOutTimeSecは考慮されません。")]
        public void stop()
        {
            foreach (var item in PlayingSourceRequestIdList)
            {
                SendRequest.stop(item);
            }
            PlayingSourceRequestIdList.Clear();
            PausingSourceRequestIdList.Clear();
        }

        [Action, DisplayName("FadeOutStop"), Description("再生中の音をフェードアウト後停止します。")]
        public void fadeOutAndStop()
        {
            if (FadeOutTimeSec <= 0.0f)
            {
                stop();
                return;
            }

            foreach (var item in PlayingSourceRequestIdList)
            {
                SendRequest.fadeOutAndStop(item, FadeOutTimeSec);
            }
            PlayingSourceRequestIdList.Clear();
            PausingSourceRequestIdList.Clear();
        }

        public void updateCurrentParameters()
        {
            _CurrentVolume = _Volume;
            _CurrentPitch = _Pitch;
            _CurrentBusId = _BusId;
            _CurrentLoop = _Loop;
            _CurrentPanningType = _PanningType;
            _CurrentEnableDistanceAttenuation = _EnableDistanceAttenuation;
            _CurrentMinDistanceAttenuation = _MinDistanceAttenuation;
            _CurrentMaxDistanceAttenuation = _MaxDistanceAttenuation;
            _CurrentAngle = _Angle;
        }

        /// <summary>
        /// Source情報更新
        /// チャンネル数に応じて再生設定を変更する
        /// </summary>
        public void updateSourceInfo()
        {
            if (_SourceAsset == null)
            {
                return;
            }

            var sourceId = SourceAccessor.getId(_SourceAsset);

            // チャンネル数を取得
            Channel = SourceAccessor.getChannels(sourceId);

            // 複数チャンネルの場合はパンニング無効
            if (Channel > 1)
            {
                _PanningType = PanningType.Direct;
            }

            // 波形の長さを取得
            DurationSec = SourceAccessor.getDurationSec(sourceId);

            // ループ設定
            _Loop = SourceAccessor.isLoopEnable(sourceId);

        }

        public void clearPlayingSourceRequestIdList()
        {
            PlayingSourceRequestIdList.Clear();
            PausingSourceRequestIdList.Clear();
        }

        #endregion
    }

    [UpdateOrder((int)UpdateOrder.SoundController)]
    public class SoundController : via.Behavior
    {
        #region Property
        public List<SoundSource> _Sources = new List<SoundSource> { new SoundSource() };

        #endregion

        #region Method

        [DisplayName("StopAll"), Description("再生中のあらゆる音を停止します"), Action]
        public void stopAll()
        {
            SendRequest.stopAll();
            foreach (var item in _Sources)
            {
                item.clearPlayingSourceRequestIdList();
            }
        }

        public override void onLoad()
        {
            foreach (var item in _Sources)
            {
                item.updateSourceInfo();
            }
        }

        public override void onUnload()
        {
            foreach (var item in _Sources)
            {
                item.stop();
            }
        }

        public override void awake()
        {
        }

        public override void start()
        {
            _cpMotion = GameObject.getSameComponent<via.motion.Motion>();
            FlowManager.Instance.beforePause += pause;
        }

        public override void editUpdate()
        {
            updateSources();
        }

        public override void update()
        {
            if (isPause)
            {
                pauseDelay -= 1;
                if (pauseDelay <= 0)
                {
                    unpause();
                }
            }
            updateSources();
        }

        public override void lateUpdate()
        {
            updateSoundTrack();
        }

        /// <summary>
        /// サウンドを再生
        /// </summary>
        /// <param name="id"></param>
        public void play(int id)
        {
            if (id == -1) return;
            int count = _Sources.Count;
            for (int i = 0; i < count; i++)
            {
                if (_Sources[i].RequestId == id)
                {
                    _Sources[i].play();
                    break;
                }
            }
        }

        /// <summary>
        /// サウンドを停止
        /// </summary>
        /// <param name="id"></param>
        public void stop(int id)
        {
            int count = _Sources.Count;
            for (int i = 0; i < count; i++)
            {
                if (_Sources[i].RequestId == id)
                {
                    _Sources[i].stop();
                    break;
                }
            }
        }

        /// <summary>
        /// フェードアウトして停止
        /// </summary>
        /// <param name="id"></param>
        public void fadeOutAndStop(int id)
        {
            int count = _Sources.Count;
            for (int i = 0; i < count; i++)
            {
                if (_Sources[i].RequestId == id)
                {
                    _Sources[i].fadeOutAndStop();
                    break;
                }
            }
        }


        void updateSources()
        {
            foreach (var item in _Sources)
            {
                item.Position = GameObject.Transform.Position;
                item.update();
            }
        }

        /// <summary>
        /// サウンドが再生中がチェックする
        /// </summary>
        public bool isPlayingSound(int soundId)
        {
            if (_Sources[soundId].PlayingSourceRequestIdList.Count != 0)
            {
                debug.infoLine("Playing");
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// 対象の流れているサウンドをすべて停止する
        /// </summary>
        public void stopAllSound(bool isFadeOut)
        {
            if(isFadeOut)
            {
                foreach (var item in _Sources)
                {
                    item.fadeOutAndStop();
                }
            }
            else
            {
                foreach (var item in _Sources)
                {
                    item.stop();
                }
            }
        }

        /// <summary>
        /// モーションコンポーネント
        /// </summary>
        via.motion.Motion _cpMotion = null;

        /// <summary>
        /// サウンドトラックの更新
        /// </summary>
        void updateSoundTrack()
        {
            if (_cpMotion == null)
            {
                // モーションコンポーネントが取得できない場合は処理を行わない
                return;
            }

            for (int layerIdx=0; layerIdx < _cpMotion.Layer.Count; layerIdx++)
            {
            var node = _cpMotion.Layer[layerIdx].HighestWeightMotionNode;
                if (node != null)
                {
                    //MotionデータからMotionSequenceに登録されたSequenceを取得する
                    var count = node.SequenceTracksCount;
                    for (uint i = 0; i < count; i++)
                    {
                        var tracktype = node.getSequenceTracksTypeinfo(i);
                        if (tracktype == typeof(blackfilter.SoundController.SoundTrack))
                        {
                            //トラックから値を取得する
                            var track = Activator.CreateInstance<blackfilter.SoundController.SoundTrack>();
                            node.getSequenceTracks(i, track);

                            if (track.SoundID1 != -1)
                            {
                                play(track.SoundID1);
                            }
                            if (track.SoundID2 != -1)
                            {
                                play(track.SoundID2);
                            }
                            if (track.SoundID3 != -1)
                            {
                                play(track.SoundID3);
                            }
                            if (track.SoundID4 != -1)
                            {
                                play(track.SoundID4);
                            }
                            if (track.isStop)
                            {
                                stop(track.SoundID1);
                                stop(track.SoundID2);
                                stop(track.SoundID3);
                                stop(track.SoundID4);
                            }
                            else if (track.isFadeOutStop)
                            {
                                fadeOutAndStop(track.SoundID1);
                                fadeOutAndStop(track.SoundID2);
                                fadeOutAndStop(track.SoundID3);
                                fadeOutAndStop(track.SoundID4);
                            }
                        }
                    }
                }
            }
        }


        bool isPause = false;
        int pauseDelay = 0;
        List<int> playingSounds = new();

        private void pause()
        {
            isPause = true;
            pauseDelay = 2;
            playingSounds.Clear();
            for (int i = 0; i < _Sources.Count; i++)
            {
                if (isPlayingSound(i))
                {
                    playingSounds.Add(i);
                    _Sources[i].pause();
                }
            }
        }

        private void unpause()
        {
            isPause = false;
            foreach (int i in playingSounds)
            {
                _Sources[i].play();
            }
        }

        public override void onDestroy()
        {
            FlowManager.Instance.beforePause -= pause;
            stopAll();
        }

        #endregion

        public class SoundTrack : via.motion.Tracks
        {
            [Keyable(InterpolationFlag.Event, InterpolationType.Event, false, false)]
            public int SoundID1 = -1;

            [Keyable(InterpolationFlag.Event, InterpolationType.Event, false, false)]
            public int SoundID2 = -1;

            [Keyable(InterpolationFlag.Event, InterpolationType.Event, false, false)]
            public int SoundID3 = -1;

            [Keyable(InterpolationFlag.Event, InterpolationType.Event, false, false)]
            public int SoundID4 = -1;

            [Keyable(InterpolationFlag.Event, InterpolationType.Event, false, false)]
            public bool isStop = false;

            [Keyable(InterpolationFlag.Event, InterpolationType.Event, false, false)]
            public bool isFadeOutStop = false;
        }


    }

}
