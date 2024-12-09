using System;
using System.Collections;
using System.Collections.Generic;
using Agava.YandexGames;
using Kimicu.YandexGames.Extension;
using Kimicu.YandexGames.Utils;
using UnityEngine;
using UnityEngine.Events;
using Coroutine = Kimicu.YandexGames.Utils.Coroutine;
using Random = UnityEngine.Random;

namespace Kimicu.YandexGames
{
    public static class Leaderboard
    {
        private const string _SAVE_NAME = "Leaderboard";
        private const string _RANDOM_PROFILE_PICTURE = "https://avatar.iran.liara.run/public";
        
        public static LeaderboardDescriptionResponse LeaderboardInfo { get; private set; }
        public static IEnumerable<LeaderboardEntryResponse> TopLeaderboardEntries { get; private set; }
        public static IEnumerable<LeaderboardEntryResponse> CompetingLeaderboardEntries { get; private set; }
        public static LeaderboardEntryResponse LeaderboardEntryUser { get; private set; }
        public static string LeaderboardId = "Leaderboard";

        public static readonly UnityEvent OnUpdateInfo = new();

        public static bool Initialized { get; private set; }

        private static readonly Coroutine UpdateRoutine = new Coroutine();

        private const int CompetingEntriesAmount = 10;
        private const int TopEntriesAmount = 20;

        public static IEnumerator Initialize(int delayUpdate = 100)
        {
            if (Initialized)
            {
                Debug.LogWarning("Leaderboard is initialized!");
                yield break;
            }

#if !UNITY_EDITOR
        if (!PlayerAccount.IsAuthorized)
        {
            Debug.LogWarning("Player is not authorized!");
            yield break;
        }
#endif

            yield return UpdateLeaderboard();

            UpdateRoutine.StartRoutine(DelayUpdate(delayUpdate));

            Initialized = true;
        }

        private static void OnError(string error) => Debug.LogError(error);

        private static IEnumerator DelayUpdate(int delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                yield return UpdateLeaderboard();
            }
        }

        public static IEnumerator UpdateLeaderboard()
        {
#if !UNITY_EDITOR
        if (!PlayerAccount.IsAuthorized)
        {
            Debug.LogWarning("Player isn't authorized!");
            yield break;
        }
#endif

            bool receivedTop = false;
            bool receivedCompeting = false;
            bool receivedPlayerEntry = false;

#if !UNITY_EDITOR
        GetTopEntries(() => { receivedTop = true; });
        yield return new WaitUntil(() => receivedTop);
        GetCompetingEntries(() => { receivedCompeting = true; });
        yield return new WaitUntil(() => receivedCompeting);
        GetPlayerEntry(() => { receivedPlayerEntry = true; });
#else
            LeaderboardEntryUser = EditorPlayerEntry();
            receivedPlayerEntry = true;
            EditorGenerateTopEntries(() => receivedTop = true);
            EditorGenerateCompetingEntries(() => receivedCompeting = true);
            yield return new WaitForSeconds(2);
#endif

            yield return new WaitUntil(() => receivedTop && receivedPlayerEntry && receivedCompeting);
            Initialized = true;
            OnUpdateInfo?.Invoke();
        }

        private static void GetTopEntries(Action onSuccess)
        {
            Agava.YandexGames.Leaderboard.GetEntries(LeaderboardId, response =>
            {
                TopLeaderboardEntries = response.entries;
                onSuccess.Invoke();
            }, OnError, TopEntriesAmount, 0);
        }

        private static void GetCompetingEntries(Action onSuccess)
        {
            Agava.YandexGames.Leaderboard.GetEntries(LeaderboardId, response =>
            {
                CompetingLeaderboardEntries = response.entries;
                LeaderboardInfo = response.leaderboard;
                onSuccess.Invoke();
            }, OnError, 0, CompetingEntriesAmount);
        }

        private static void GetPlayerEntry(Action onSuccess)
        {
            Agava.YandexGames.Leaderboard.GetPlayerEntry(LeaderboardId, response =>
            {
                LeaderboardEntryUser = response;
                onSuccess.Invoke();
            }, OnError);
        }

        public static void SetScore(int score, Action onSuccessCallback = null, Action<string> onErrorCallback = null, string extraData = "")
        {
#if UNITY_EDITOR
            var editorPlayerEntry = LeaderboardEntryUser;
            editorPlayerEntry.score = score;
            
            FileExtensions.SaveObject(_SAVE_NAME, editorPlayerEntry);
            onSuccessCallback?.Invoke();
            return;
#endif
            if (PlayerAccount.IsAuthorized)
            {
                Agava.YandexGames.Leaderboard.SetScore(LeaderboardId, score,
                    () =>
                    {
                        onSuccessCallback?.Invoke();
                        Coroutines.StartRoutine(UpdateLeaderboard());
                    }, onErrorCallback, extraData);
            }
            else
            {
                onErrorCallback?.Invoke("Player is not auth!");
            }
        }

        #region Editor

#if UNITY_EDITOR
        private static void EditorGenerateTopEntries(Action onSuccess)
        {
            var entriesResponse = new LeaderboardEntryResponse[TopEntriesAmount];

            int randomScoreMultiplier = Random.Range(1500, 2500);

            for (int i = 0; i < TopEntriesAmount; i++)
            {
                bool generateAvatar = Random.Range(0, 10) != 1;

                entriesResponse[i] = new LeaderboardEntryResponse
                {
                    rank = i + 1,
                    score = randomScoreMultiplier * (TopEntriesAmount - i),
                    extraData = "example description",
                    formattedScore = (randomScoreMultiplier * (TopEntriesAmount - i)).ToString("D"),
                    player = new PlayerAccountProfileDataResponse
                    {
                        publicName = $"user {Random.Range(100_000_000, 999_999_999)}",
                        profilePicture = generateAvatar ? _RANDOM_PROFILE_PICTURE : "",
                        lang = "ru",
                        scopePermissions = new PlayerAccountProfileDataResponse.ScopePermissions
                        {
                            avatar = generateAvatar ? _RANDOM_PROFILE_PICTURE : "",
                            public_name = "example name"
                        },
                        uniqueID = Guid.NewGuid().ToString()
                    }
                };
            }

            TopLeaderboardEntries = entriesResponse;

            onSuccess.Invoke();
        }

        private static void EditorGenerateCompetingEntries(Action onSuccess)
        {
            var entriesResponse = new LeaderboardEntryResponse[CompetingEntriesAmount * 2 + 1];

            for (int i = 0; i < CompetingEntriesAmount; i++) // generate entries above
            {
                bool generateAvatar = Random.Range(0, 10) != 1;

                entriesResponse[i] = new LeaderboardEntryResponse
                {
                    rank = LeaderboardEntryUser.rank - (CompetingEntriesAmount - i),
                    score = LeaderboardEntryUser.score + 1,
                    extraData = "example description",
                    formattedScore = (LeaderboardEntryUser.score + 1).ToString("D"),
                    player = new PlayerAccountProfileDataResponse
                    {
                        publicName = $"user {Random.Range(100_000_000, 999_999_999)}",
                        profilePicture = generateAvatar ? _RANDOM_PROFILE_PICTURE : "",
                        lang = "ru",
                        scopePermissions = new PlayerAccountProfileDataResponse.ScopePermissions
                        {
                            avatar = generateAvatar ? _RANDOM_PROFILE_PICTURE : "",
                            public_name = "example name"
                        },
                        uniqueID = Guid.NewGuid().ToString()
                    }
                };
            }

            entriesResponse[CompetingEntriesAmount] = LeaderboardEntryUser;

            for (int i = CompetingEntriesAmount + 1; i < entriesResponse.Length; i++) // generate entries above
            {
                bool generateAvatar = Random.Range(0, 10) != 1;

                entriesResponse[i] = new LeaderboardEntryResponse
                {
                    rank = LeaderboardEntryUser.rank + i,
                    score = LeaderboardEntryUser.score - 1,
                    extraData = "example description",
                    formattedScore = (LeaderboardEntryUser.score - 1).ToString("D"),
                    player = new PlayerAccountProfileDataResponse
                    {
                        publicName = $"user {Random.Range(100_000_000, 999_999_999)}",
                        profilePicture = generateAvatar ? _RANDOM_PROFILE_PICTURE : "",
                        lang = "ru",
                        scopePermissions = new PlayerAccountProfileDataResponse.ScopePermissions
                        {
                            avatar = generateAvatar ? _RANDOM_PROFILE_PICTURE : "",
                            public_name = "example name"
                        },
                        uniqueID = Guid.NewGuid().ToString()
                    }
                };
            }

            CompetingLeaderboardEntries = entriesResponse;
            onSuccess.Invoke();
        }
        
        private static LeaderboardEntryResponse EditorPlayerEntry()
        {
            int score = Random.Range(10, 20);

            var editorEntry = new LeaderboardEntryResponse
            {
                rank = Random.Range(10000, 20000),
                score = score,
                extraData = "example description",
                formattedScore = score.ToString("D"),
                player = new PlayerAccountProfileDataResponse
                {
                    publicName = "OurPlayerName",
                    profilePicture = _RANDOM_PROFILE_PICTURE,
                    lang = "ru",
                    scopePermissions = new PlayerAccountProfileDataResponse.ScopePermissions
                    {
                        avatar = "",
                        public_name = "OurPlayerName"
                    },
                    uniqueID = "userPlayer"
                }
            };
            
            return FileExtensions.LoadObject(_SAVE_NAME, editorEntry);
        }

#endif

        #endregion
    }
}