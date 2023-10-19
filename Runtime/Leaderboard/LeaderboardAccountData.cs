using System;
using Agava.YandexGames;
using UnityEngine;

namespace Kimicu.YandexGames
{
    [Serializable]
    public class LeaderboardAccountData
    {
        public string UserName { get; private set; }
        public string UserID { get; private set; }
        public string Language { get; private set; }
        public Sprite Picture { get; set; }
        public string PictureURL { get; private set; }

        public int Rank { get; private set; }
        public int Score { get; private set; }
        public string UserDescription { get; private set; }
        public string FormattedScore { get; private set; }

        public LeaderboardAccountData(PlayerAccountProfileDataResponse user, Sprite picture, int rank, int score,
            string description,
            string formattedScore)
        {
            UserName = user.publicName;
            Language = user.lang;
            UserID = user.uniqueID;
            PictureURL = user.profilePicture;
            Picture = picture;
            Rank = rank;
            Score = score;
            UserDescription = description;
            FormattedScore = formattedScore;
        }
    }
}