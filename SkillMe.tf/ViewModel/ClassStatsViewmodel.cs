using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using SkillMe.tf.Models;

namespace SkillMe.tf.ViewModel
{
    public class ClassStatsViewmodel : ViewModelBase, INotifyPropertyChanged
    {
        public SteamId PlayerId { get; set; }

        public Classes PlayerClass { get; set; }
       
        public int MatchSpread { get; set; }

        private float _avgDa;
        public float AvgDa
        {
            get { return _avgDa; }
            set
            {
                _avgDa = value;
                OnPropertyChanged();
            }
        }

        private float _avgK;
        public float AvgK
        {
            get { return _avgK; }
            set
            {
                _avgK = value;
                OnPropertyChanged();
            }
        }

        private float _avgD;
        public float AvgD
        {
            get { return _avgD; }
            set
            {
                _avgD = value;
                OnPropertyChanged();
            }
        }

        private float _avgKd;
        public float AvgKd
        {
            get { return _avgKd; }
            set
            {
                _avgKd = value;
                OnPropertyChanged();
            }
        }

        private float _avgKad;
        public float AvgKad
        {
            get { return _avgKad; }
            set
            {
                _avgKad = value;
                OnPropertyChanged();
            }
        }

        public ClassStatsViewmodel(SteamId playerId, Classes playerClass)
        {
            PlayerId = playerId;
            PlayerClass = playerClass;
        }

        public void AverageMatch(JObject match)
        {
            //For older logs
            JToken playerInfo = match["players"][PlayerId.NormalId];

            if (playerInfo == null)
            {
                //For newer logs
                playerInfo = match["players"][PlayerId.Id3];

                if (playerInfo == null)
                {
                    return;
                }
            }

            Classes playedClass = ClassesHelper.FromString(playerInfo["class_stats"][0]["type"].ToString());
            if (PlayerClass == Classes.All)
            {
                AvgDa = AverageStat(AvgDa, (int)playerInfo["dmg"]);
                AvgK = AverageStat(AvgK, (int)playerInfo["kills"]);
                AvgD = AverageStat(AvgD, (int)playerInfo["deaths"]);
                AvgKd = AverageStat(AvgKd, (float)playerInfo["kpd"]);
                AvgKad = AverageStat(AvgKad, (float)playerInfo["kapd"]);

                MatchSpread += 1;
            }
            else if (playedClass.Equals(PlayerClass))
            {
                MatchSpread += 1;
            }
        }

        public float AverageStat(float stat, float addition)
        {
            return ((stat * MatchSpread) + addition) / (MatchSpread + 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
