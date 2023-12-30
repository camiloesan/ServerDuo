using NAudio.Wave;

namespace ClienteDuo.Utilities
{
    public class MusicManager
    {
        private readonly WaveOutEvent _waveOutEvent;
        private bool _isMusicEnabled = true;
        private float _volume = 0.5f;

        public MusicManager(string musicFilePath)
        {
            var audioFileReader = new AudioFileReader(musicFilePath);
            _waveOutEvent = new WaveOutEvent();
            _waveOutEvent.Init(audioFileReader);
        }

        public void ToggleMusic()
        {
            _isMusicEnabled = !_isMusicEnabled;

            if (_isMusicEnabled)
            {
                PlayMusic();
            }
            else
            {
                StopMusic();
            }
        }

        public static void PlayCardFlippedSound()
        {
            var audioFileReader = new AudioFileReader("SFX\\cardFlippedSound.wav");
            var waveOut = new WaveOutEvent();
            waveOut.Init(audioFileReader);
            waveOut.Volume = 0.5f;
            waveOut.Play();
        }

        public static void PlayMatchFinishedSound()
        {
            var audioFileReader = new AudioFileReader("SFX\\matchFinishedSound.wav");
            var waveOut = new WaveOutEvent();
            waveOut.Init(audioFileReader);
            waveOut.Volume = 0.5f;
            waveOut.Play();
        }

        public static void PlayPlayerJoinedSound()
        {
            var audioFileReader = new AudioFileReader("SFX\\playerJoinedSound.wav");
            var waveOut = new WaveOutEvent();
            waveOut.Init(audioFileReader);
            waveOut.Volume = 0.5f;
            waveOut.Play();
        }

        public static void PlayPlayerLeftSound()
        {
            var audioFileReader = new AudioFileReader("SFX\\playerLeftSound.wav");
            var waveOut = new WaveOutEvent();
            waveOut.Init(audioFileReader);
            waveOut.Volume = 0.5f;
            waveOut.Play();
        }

        private void PlayMusic()
        {
            if (!_isMusicEnabled) return;
            _waveOutEvent.Volume = _volume;
            _waveOutEvent.Play();
        }

        private void StopMusic()
        {
            _waveOutEvent.Stop();
        }

        public float Volume
        {
            get => _volume;
            set
            {
                if (value < 0) _volume = 0f;
                else if (value > 1) _volume = 1f;
                else _volume = value;

                if (_isMusicEnabled)
                {
                    _waveOutEvent.Volume = _volume;
                }
            }
        }

        public bool IsMusicEnabled => _isMusicEnabled;
    }
}
