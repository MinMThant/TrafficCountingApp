using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.VFW;
using AForge.Vision.Motion;


namespace TrafficCountingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MotionDetector _motionDetector;
        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            string fileName = @"D:\work\Thein.Net\TrafficCountingApp\TrafficCountingApp\Videos\Bunny.avi";

            _motionDetector = MotionDetectorIni();
            VideoReadingProcessDirectShow(fileName);
            // VideoReadingProcessAVIReader(fileName);
        }

        public MotionDetector MotionDetectorIni()
        {
            // create motion detector
            MotionDetector detector = new MotionDetector(
                new SimpleBackgroundModelingDetector(),
                new MotionAreaHighlighting());
            return detector;
        }

        public void VideoReadingProcessDirectShow(string fileName)
        {
            // See: http://www.aforgenet.com/forum/viewtopic.php?f=2&t=728
            // create video source
            FileVideoSource videoSource = new FileVideoSource(fileName);
            // set NewFrame event handler
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            // start the video source
            videoSource.Start();
        }
        
         

        // New frame event handler, which is invoked on each new available video frame
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap videoFrame = eventArgs.Frame;
            // process the frame
            if (_motionDetector.ProcessFrame(videoFrame) > 0.02)
            {
                // There is a difference. process the frame somehow or display it.
                Console.WriteLine("This file is different.");
            }
        }

        [System.Obsolete("AVIReader is not working properly unless you install drivers")]
        /*
         * Not tested yet. 
            What I did are
            1. Install latest ffdshow for 32bit and 64bit environment.
            You can search for it by "ffdshow 32bit download" or "ffdshow 64bit download" on google.
            2. Download and install Win7DSFilterTweaker for below link to change ffdshow as a defalut codec. 
            http://www.hack7mc.com/2009/04/replacin ... yback.html
            3. Execute Win7DSFilterTweaker and set ffdshow as a default codec.

            After 3, I found Open method in AVIReader doesn't fail anymore.

         */
        public void VideoReadingProcessAVIReader(string fileName)
        {
         
            // instantiate AVI reader
            AVIReader reader = new AVIReader();

            // open video file
            //TODO: OpenFileDialog later.
            //openFileDialog1.ShowDialog();
            //aviReader.Open(openFileDialog1.FileName);   //there I have exception
            //Int32 length = aviReader.Length;
            
            reader.Open(fileName);
            // read the video file
            // continuously feed video frames to motion detector

            while (reader.Position - reader.Start < reader.Length)
            {
                // get next frame
                var videoFrame = reader.GetNextFrame();
                

                if (_motionDetector.ProcessFrame(videoFrame) > 0.02)
                {
                    // There is a difference. process the frame somehow or display it.
                    Console.WriteLine("This file is different.");   
                }
            }
            reader.Close();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                    lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                lblStatus.Content = "No file selected...";
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
        }

        //Testing
        //Task<bool> task = Test_int();
        //bool x = await task;
        private static async Task<bool> Test_int()
        {
            return true;
        }
    }
}
