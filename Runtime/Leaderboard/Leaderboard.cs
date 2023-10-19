using System;
using System.Collections;
using System.Collections.Generic;
using Agava.YandexGames;
using KimicuUtility;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using Random = UnityEngine.Random;
#endif

namespace Kimicu.YandexGames
{
    /// <summary> DEMO: Могут возникнуть ошибки, будьте осторожны! </summary>
    public static class Leaderboard
    {
        public static LeaderboardDescriptionResponse LeaderboardInfo { get; private set; }
        public static IEnumerable<LeaderboardEntryResponse> LeaderboardEntries { get; private set; }
        public static IEnumerable<LeaderboardGetEntriesResponse.Range> LeaderboardRanges { get; private set; }
        public static LeaderboardAccountData User { get; private set; }

        public static readonly UnityEvent OnUpdateInfo = new();

        private static bool _available;
        private static bool _initialize;
        private static readonly KiCoroutine UpdateRoutine = new();

        public static IEnumerator Initialize()
        {
            if (_initialize)
            {
                Debug.LogWarning("Leaderboard is initialized!");
                yield break;
            }

            KimicuYandexSettings settings = KimicuYandexSettings.Instance;
            if (!settings.LeaderboardActive) yield break;
            _available = settings.LeaderboardActive;

            yield return UpdateLeaderboard();

            UpdateRoutine.StartRoutine(DelayUpdate(settings.DelayUpdateLeaderboardInfo), true);

            _available = true;
            _initialize = true;
        }

        #region Editor

        #if UNITY_EDITOR
        private static void GenerateEntriesInEditor(string leaderboardName,
            Action<LeaderboardGetEntriesResponse> onSuccessCallback, int topPlayerCount, int competingPlayerCount,
            bool includeSelf, ProfilePictureSize pictureSize)
        {
            var entriesResponse = new LeaderboardEntryResponse[topPlayerCount];
            for (int i = 0; i < topPlayerCount; i++)
            {
                entriesResponse[i] = new LeaderboardEntryResponse
                {
                    rank = topPlayerCount - i + 1,
                    score = 100 * (topPlayerCount - i + 1),
                    extraData = "example description player",
                    formattedScore = (10 * i).ToString("D"),
                    player = new PlayerAccountProfileDataResponse
                    {
                        publicName = $"user {Random.Range(100_000_000, 999_999_999)}",
                        profilePicture =
                            "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png",
                        lang = "ru",
                        scopePermissions = new PlayerAccountProfileDataResponse.ScopePermissions
                        {
                            avatar =
                                "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png",
                            public_name = "example name"
                        },
                        uniqueID = Guid.NewGuid().ToString()
                    }
                };
            }

            var ranges = new LeaderboardGetEntriesResponse.Range[topPlayerCount];
            for (int i = 0; i < topPlayerCount - 1; i++)
            {
                ranges[i] = new LeaderboardGetEntriesResponse.Range()
                {
                    start = entriesResponse[i].rank,
                    size = Mathf.Abs(entriesResponse[i].rank - entriesResponse[i + 1].rank)
                };
            }

            onSuccessCallback?.Invoke(new LeaderboardGetEntriesResponse
            {
                entries = entriesResponse,
                leaderboard = new LeaderboardDescriptionResponse
                {
                    description = new LeaderboardDescriptionResponse.Description()
                    {
                        score_format = new LeaderboardDescriptionResponse.Description.ScoreFormat()
                        {
                            options = new LeaderboardDescriptionResponse.Description.ScoreFormat.Options()
                            {
                                decimal_offset = 0
                            }
                        },
                        invert_sort_order = KimicuYandexSettings.Instance.InvertSortOrder,
                        type = KimicuYandexSettings.Instance.LeaderboardValueType.ToString()
                    },
                    title = new LeaderboardDescriptionResponse.Title
                    {
                        en = KimicuYandexSettings.Instance.LeaderboardName,
                        ru = KimicuYandexSettings.Instance.LeaderboardName
                    },
                    appID = "yandexKimicuId",
                    @default = true,
                    name = KimicuYandexSettings.Instance.LeaderboardName
                },
                userRank = KimicuYandexSettings.Instance.PlayerRankInEditor,
                ranges = ranges
            });
        }

        private static void GeneratePlayerEntryInEditor(string leaderboardName,
            Action<LeaderboardEntryResponse> onSuccessCallback, ProfilePictureSize pictureSize)
        {
            int rank = Random.Range(1, 10);
            onSuccessCallback?.Invoke(new LeaderboardEntryResponse
            {
                rank = rank,
                score = 100 * rank,
                formattedScore = (100 * rank).ToString("D"),
                extraData = "example description player",
                player = new PlayerAccountProfileDataResponse
                {
                    publicName = $"user {Random.Range(100_000_000, 999_999_999)}",
                    profilePicture =
                        "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png",
                    lang = "ru",
                    scopePermissions = new PlayerAccountProfileDataResponse.ScopePermissions
                    {
                        avatar =
                            "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png",
                        public_name = "example name"
                    },
                    uniqueID = Guid.NewGuid().ToString()
                }
            });
        }
        #endif

        #endregion

        private static void OnError(string error) => Debug.LogWarning(error);

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
            if (!_available) yield break;
            KimicuYandexSettings settings = KimicuYandexSettings.Instance;
            bool receivedRecords = false;
            bool receivedPlayerEntry = false;
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Leaderboard.GetEntries(settings.LeaderboardName,
                response =>
                {
                    receivedRecords = true;
                    LeaderboardInfo = response.leaderboard;
                    LeaderboardEntries = response.entries;
                    LeaderboardRanges = response.ranges;
                }, OnError,
                settings.TopPlayersCount, settings.CompetingPlayersCount, settings.IncludeSelf, settings.PictureSize);
            #elif UNITY_EDITOR
            GenerateEntriesInEditor(settings.LeaderboardName, response =>
            {
                receivedRecords = true;
                LeaderboardInfo = response.leaderboard;
                LeaderboardEntries = response.entries;
                LeaderboardRanges = response.ranges;
            }, settings.TopPlayersCount, settings.CompetingPlayersCount, settings.IncludeSelf, settings.PictureSize);
            #else
            receivedRecords = true;
            #endif
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Leaderboard.GetPlayerEntry(settings.LeaderboardName,
                response =>
                {
                    User = new LeaderboardAccountData(response.player, null, response.rank, response.score,
                        response.extraData, response.formattedScore);
                    KiCoroutine routine = new();
                    routine.StartRoutine(response.player.profilePicture.GetPicture(texture =>
                    {
                        User.Picture = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                            Vector2.one / 2);
                        receivedPlayerEntry = true;
                        routine.StopRoutine();
                    }, error =>
                    {
                        receivedPlayerEntry = true;
                        OnError(error);
                    }));
                    if (!settings.WaitInitializePicture) receivedPlayerEntry = true;
                }, OnError, settings.PictureSize);
            #elif UNITY_EDITOR
            GeneratePlayerEntryInEditor(settings.LeaderboardName, response =>
            {
                User = new LeaderboardAccountData(response.player, null, response.rank, response.score,
                    response.extraData, response.formattedScore);
                KiCoroutine routine = new();
                routine.StartRoutine(response.player.profilePicture.GetPicture(texture =>
                {
                    User.Picture = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        Vector2.one / 2);
                    receivedPlayerEntry = true;
                    routine.StopRoutine();
                }, error =>
                {
                    receivedPlayerEntry = true;
                    OnError(error);
                }));
                if (!settings.WaitInitializePicture) receivedPlayerEntry = true;
            }, settings.PictureSize);
            #else
            receivedPlayerEntry = true;
            #endif
            yield return new WaitUntil(() => receivedRecords && receivedPlayerEntry);
            OnUpdateInfo?.Invoke();
        }

        public static void SetScore(int score, Action onSuccessCallback = null, Action<string> onErrorCallback = null,
            string extraData = "")
        {
            if (!_available)
            {
                onErrorCallback?.Invoke("Leaderboard is off!");
                return;
            }

            if (PlayerAccount.IsAuthorized)
            {
                Agava.YandexGames.Leaderboard.SetScore(KimicuYandexSettings.Instance.LeaderboardName, score,
                    onSuccessCallback, onErrorCallback, extraData);
            }
            else
            {
                onErrorCallback?.Invoke("Player is not auth!");
            }
        }

        public static IEnumerator SetScore(int score, Action<string> onErrorCallback = null,
            string extraData = "")
        {
            if (!_available)
            {
                onErrorCallback?.Invoke("Leaderboard is off!");
                yield break;
            }

            if (PlayerAccount.IsAuthorized)
            {
                bool isSuccess = false;
                bool isError = false;
                Agava.YandexGames.Leaderboard.SetScore(KimicuYandexSettings.Instance.LeaderboardName, score,
                    () => isSuccess = true, (error) =>
                    {
                        Debug.LogWarning(error);
                        onErrorCallback?.Invoke(error);
                        isError = true;
                    }, extraData);

                yield return new WaitUntil(() => isSuccess || isError);
            }

            onErrorCallback?.Invoke("Player is not auth!");
        }
    }
}