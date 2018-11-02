using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json.Linq;
using SkillMe.tf.Models;

namespace SkillMe.tf.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private const string _webUrl = "https://logs.tf/api/v1/";
        private HttpClient _client = new HttpClient();

        private SteamId _internalId;
        private List<JObject> _matches = new List<JObject>();

        private string _folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        private ClassStatsViewmodel _currentClassView;
        public ClassStatsViewmodel CurrentClassView
        {
            get { return _currentClassView; }
            set
            {
                _currentClassView = value;
                OnPropertyChanged();
            }
        }

        private string _steamID;
        public string SteamID
        {
            get { return _steamID; }
            set
            {
                _steamID = value;
                CheckSteamID();
            }
        }

        private bool _canRetrieve;
        public bool CanRetrieve
        {
            get { return _canRetrieve; }
            set
            {
                _canRetrieve = value;
                OnPropertyChanged();
            }
        }

        private int _amountOfMatches;
        public int AmountOfMatches
        {
            get { return _amountOfMatches; }
            set
            {
                _amountOfMatches = value;
                OnPropertyChanged();
            }
        }

        private int _amountLoaded;
        public int AmountLoaded
        {
            get { return _amountLoaded; }
            set
            {
                _amountLoaded = value;
                OnPropertyChanged();
            }
        }

        public ICommand RetrieveInformationCommand { get; }

        public MainViewModel()
        {
            Directory.CreateDirectory(@_folder + "/matches");

            _client.Timeout = TimeSpan.FromMinutes(30);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri(@_webUrl);

            AmountOfMatches = 0;
            AmountLoaded = 0;

            RetrieveInformationCommand = new RelayCommand(RetrieveInformation);
        }

        private void CheckSteamID()
        {
            if (SteamID.Length == 17 && SteamID.Contains("7656119"))
            {
                CanRetrieve = true;
                Directory.CreateDirectory(@_folder + "/matches/" + SteamID);

                _internalId = new SteamId(SteamID);
                CurrentClassView = new ClassStatsViewmodel(_internalId, Classes.All);

                LoadOfflineMatches();
            }
            else
            {
                CanRetrieve = false;
            }
        }

        private void LoadOfflineMatches()
        {
            DirectoryInfo info = new DirectoryInfo(@_folder + "/matches/" + SteamID);
            foreach (var matchFile in info.GetFiles("*.json"))
            {
                string json = File.ReadAllText(matchFile.FullName);
                JObject match = JObject.Parse(json);
                _matches.Add(match);

                CurrentClassView.AverageMatch(match);

                AmountOfMatches += 1;
                AmountLoaded += 1;
            }
        }

        private async void RetrieveInformation()
        {
            string jsoninformation = await GetMatchesInformation();

            if (jsoninformation != null)
            {
                JObject json = JObject.Parse(jsoninformation);

                int OldAmountOfMatches = AmountOfMatches;
                AmountOfMatches = (int)json["results"];

                //No loading needed incase everything is offline.
                if (OldAmountOfMatches == AmountOfMatches) return;

                MessageBoxResult dialogResult = MessageBox.Show("This can take some time. A pause of 3 seconds every 25 logs is required so the request does not time out. This means you'll have to wait about " + (((AmountOfMatches - OldAmountOfMatches) / 25) * 3) / 60 + " minutes.\n\nContinue?", "Warning", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    CanRetrieve = false;

                    for (int i = AmountLoaded; i < json["logs"].Reverse().Count(); ++i)
                    {
                        int id = (int)json["logs"][i]["id"];
                        JObject match = JObject.Parse(await GetMatchInformation(id));

                        AmountLoaded += 1;

                        CurrentClassView.AverageMatch(match);

                        //Requests will crash if no pauses.
                        if (AmountLoaded % 25 == 0)
                        {
                            System.Threading.Thread.Sleep(3000);
                        }

                        _matches.Add(match);
                        SaveMatch(id, match);
                    }
                }
            }
        }

        private void SaveMatch(int id, JObject match)
        {
            try
            {
                File.WriteAllText(@_folder + "/matches/" + SteamID + "/" + id + ".json", match.ToString());
            }
            catch (UnauthorizedAccessException exception)
            {
                //Ignore.
            }
        }

        private Task<string> GetMatchesInformation()
        {
            try
            {
                Task<String> result = _client.GetStringAsync("log?player=" + SteamID + "&limit=10000");
                return result;
            }
            catch (WebException exception)
            {
                return null;
            }
        }

        private Task<string> GetMatchInformation(int id)
        {
            try
            {
                Task<String> result = _client.GetStringAsync("log/" + id);
                return result;
            }
            catch (WebException exception)
            {
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}