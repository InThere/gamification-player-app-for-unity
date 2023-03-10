using System;
using System.Collections;
using System.Linq;
using GamificationPlayer.DTO.ExternalEvents;
using UnityEngine;
using UnityEngine.Networking;

namespace GamificationPlayer
{
    /// <summary>
    /// Represents a method that is called when the device flow is started.
    /// </summary>
    /// <param name="loginUrl">The URL where the user can login via a different device.</param>
    public delegate void StartDeviceFlowCallback(string loginUrl);

    /// <summary>
    /// Represents a method that is called when there is a new loginURL.
    /// </summary>
    /// <param name="loginToken">The loginURL that can be used to log in the user in the gamification player.</param>
    public delegate void OnUserLoggedInEvent(string redirectURL);

    /// <summary>
    /// Represents a method that is called when a module is started.
    /// </summary>
    /// <param name="moduleIdentifier">The module identifier of the started module.</param>
    public delegate void OnModuleStartEvent(Guid moduleIdentifier);

    /// <summary>
    /// Represents a method that is called when a page is loaded.
    /// After this method is called, it can be checked if the user is logged in.
    /// </summary>
    public delegate void OnPageViewEvent();

    /// <summary>
    /// Represents a method that is called when an external event is fired by the gamification player.
    /// Most of these events are handled by this package. This method can be used to check for custom events, such as the "quitEvent".
    /// </summary>
    /// <param name="eventType">The type of event that is being called.</param>
    public delegate void OnExternalEvent(string eventType);

    /// <summary>
    /// Represents a method that is called when the server time is received.
    /// </summary>
    /// <param name="dateTime">The date and time of the server.</param>
    public delegate void OnServerTimeEvent(DateTime dateTime);

    /// <summary>
    /// Represents a method that is called when a fitness content is opened via the gamification player.
    /// </summary>
    /// <param name="identifier">The identifier of the fitness content.</param>
    public delegate void OnFitnessContentOpenedEvent(string identifier);

    /// <summary>
    /// Represents a method that is called when a MicroGame is opened via the gamification player.
    /// </summary>
    /// <param name="identifier">The identifier of the MicroGame.</param>
    public delegate void OnMicroGameOpenedEvent(MicroGamePayload microGame);

    /// <summary>
    /// Represents a method that is called when the language is changed or first time that the languages is set
    /// </summary>
    /// <param name="identifier">The identifier of the languages.</param>
    public delegate void OnLanguageSetEvent(string identifier);

    public class GamificationPlayerManager : MonoBehaviour
    {
        /// <summary>
        /// Occurs when there is a new login token.
        /// </summary>
        public static event OnUserLoggedInEvent OnUserLoggedIn;

        /// <summary>
        /// Occurs when a module is started.
        /// </summary>
        public static event OnModuleStartEvent OnModuleStart;

        /// <summary>
        /// Occurs when a page is loaded of the gamification player.
        /// </summary>
        public static event OnPageViewEvent OnPageView;

        /// <summary>
        /// Occurs when an external event is fired by the gamification player.
        /// </summary>
        public static event OnExternalEvent OnEvent;

        /// <summary>
        /// Occurs when the server time is received.
        /// </summary>
        public static event OnServerTimeEvent OnServerTime;

        /// <summary>
        /// Occurs when a fitness content is opened via the gamification player.
        /// </summary>
        public static event OnFitnessContentOpenedEvent OnFitnessContentOpened;

        /// <summary>
        /// Occurs when a MicroGame is opened via the gamification player.
        /// </summary>
        public static event OnMicroGameOpenedEvent OnMicroGameOpened;

        /// <summary>
        /// Occurs when a language is changed or first time that the languages is set
        /// </summary>
        public static event OnLanguageSetEvent OnLanguageSet;

        private static GamificationPlayerManager instance;
        
        /// <summary>
        /// Clears the non-persistence database and configures the class to use the mock server settings and mock requests for the mock server (GET requests instead of PATCH requests).
        /// </summary>
        public static void UseMockServer()
        {
            instance.GUseMockServer();
        }

        /// <summary>
        /// Determines whether an user is currently logged in in the gamification player.
        /// </summary>
        /// <returns>true if an user is logged in; otherwise, false.</returns>
        public static bool IsUserActive()
        {
            return instance.GIsUserActive();
        }

        /// <summary>
        /// Attempts to get the current language
        /// </summary>
        /// <param name="language">The identifier of the current language, if it is available.</param>
        /// <returns>true if the current language was successfully retrieved; otherwise, false.</returns>
        public static bool TryGetCurrentLanguage(out string language)
        {
            return instance.GTryGetCurrentLanguage(out language);
        }

        /// <summary>
        /// Attempts to get the identifier of the active user.
        /// </summary>
        /// <param name="id">The identifier of the active user, if it is available.</param>
        /// <returns>true if the active user's identifier was successfully retrieved; otherwise, false.</returns>
        public static bool TryGetActiveUserId(out Guid id)
        {
            return instance.GTryGetActiveUserId(out id);
        }

        /// <summary>
        /// Determines whether a module session is currently active.
        /// </summary>
        /// <returns>true if a module session is active; otherwise, false.</returns>
        public static bool IsModuleSessionActive()
        {
            return instance.GIsModuleSessionActive();
        }

        /// <summary>
        /// Attempts to get the identifier of the latest active module session.
        /// </summary>
        /// <param name="id">The identifier of the latest active module session, if it is available.</param>
        /// <returns>true if the latest active module session's identifier was successfully retrieved; otherwise, false.</returns>
        public static bool TryGetActiveModuleId(out Guid id)
        {
            return instance.GTryGetActiveModuleId(out id);
        }

        /// <summary>
        /// Attempts to end the latest module session.
        /// </summary>
        /// <param name="score">The score of the module session.</param>
        /// <param name="isCompleted">Indicates whether the module session was completed. The module can be ended without completing if the user ends before the end.</param>
        /// <param name="onDone">An optional callback that will be called when the operation is completed. If the operation is successful, the callback will be called without any arguments. If the operation fails, the callback will be called without any arguments.</param>
        public static void EndLatestModuleSession(int score, bool isCompleted, Action onDone = null)
        {
            instance.GEndLatestModuleSession(score, isCompleted, onDone);
        }

        /// <summary>
        /// Starts the device flow and calls the specified callback when it is started.
        /// </summary>
        /// <param name="onStart">The callback to be called when the device flow is started.</param>
        public static void StartDeviceFlow(StartDeviceFlowCallback onStart)
        {
            instance.GStartDeviceFlow(onStart);
        }

        public static bool IsDeviceFlowActive()
        {
            return instance.isDeviceFlowActive;
        }

        /// <summary>
        /// Stops the device flow
        /// </summary>
        public static void StopDeviceFlow()
        {
            instance.GStopDeviceFlow();
        }

        /// <summary>
        /// Processes an external message in JSON format.
        /// </summary>
        /// <param name="jsonMessage">The JSON message to be processed.</param>
        public static void ProcessExternalMessage(string jsonMessage)
        {
            instance.GProcessExternalMessage(jsonMessage);
        }

        /// <summary>
        /// Attempts to get the current server time.
        /// </summary>
        /// <param name="dateTime">The current server time, if it is available.</param>
        /// <returns>true if the server time was successfully retrieved; otherwise, false.</returns>
        public static bool TryGetServerTime(out DateTime dateTime)
        {
            return instance.GTryGetServerTime(out dateTime);
        }

        /// <summary>
        /// Gets the current server time and raises the OnServerTime when it is received.
        /// </summary>
        public static void GetServerTime()
        {
            instance.GGetServerOffSetTime();
        }

        /// <summary>
        /// Attempts to get the identifier of the latest MicroGame.
        /// </summary>
        /// <param name="microGamePayload">The identifier of the latest MicroGame, if it is available.</param>
        /// <returns>true if the latest MicroGame's identifier was successfully retrieved; otherwise, false.</returns>
        public static bool TryGetLatestMicroGamePayload(out MicroGamePayload microGamePayload)
        {
            return instance.GTryGetLatestMicroGamePayload(out microGamePayload);
        }

        /// <summary>
        /// Attempts to get the identifier of the latest fitness content.
        /// </summary>
        /// <param name="identifier">The identifier of the latest fitness content, if it is available.</param>
        /// <returns>true if the latest fitness content's identifier was successfully retrieved; otherwise, false.</returns>
        public static bool TryGetLatestFitnessContentIdentifier(out string identifier)
        {
            return instance.GTryGetLatestFitnessContentIdentifier(out identifier);
        }

        public static bool TryGetLatestSubdomain(out string subdomain)
        {
            return instance.GTryGetLatestSubdomain(out subdomain);
        }

        [SerializeField]
        private bool checkServerTimeOnStartUp = false;

        [SerializeField]
        [Header("Mock settings for testing")]
        private GamificationPlayerEnviromentConfig gamificationPlayerMockConfig;

        protected GamificationPlayerEndpoints gamificationPlayerEndpoints;

        protected SessionLogData sessionData;

        private bool isDeviceFlowActive;

        public void Awake()
        {
            if(instance != null)
            {
                Destroy(this);

                return;
            }

            DontDestroyOnLoad(this);

            instance = this;

            sessionData = new SessionLogData();

            gamificationPlayerEndpoints = new GamificationPlayerEndpoints(GamificationPlayerConfig.EnviromentConfig, sessionData);
            
            if(checkServerTimeOnStartUp)
            {
                GGetServerOffSetTime();
            }
        }

#if UNITY_WEBGL
        public void Start()
        {
            if(!string.IsNullOrEmpty(Application.absoluteURL) && 
                Application.absoluteURL.Contains("moduleData"))
            {
                Uri url = new Uri(Application.absoluteURL);
                var query = System.Web.HttpUtility.ParseQueryString(url.Query);
                string jwt = query["moduleData"];

                var json = JWTHelper.GetJSONWebTokenPayload(jwt, GamificationPlayerConfig.EnviromentConfig.JSONWebTokenSecret);

                var dto = json.FromJson<MicroGamePayload>();

                sessionData.AddToLog(dto);

                if(sessionData.TryGetLatestModuleId(out Guid id))
                {
                    InvokeModuleStart(id);
                }

                if(sessionData.TryGetLatestMicroGamePayload(out MicroGamePayload microGamePayload))
                {
                    InvokeMicroGameOpened(microGamePayload);
                }
            }
        }
#endif

        private bool GTryGetServerTime(out DateTime dateTime)
        {
            dateTime = DateTime.Now;

            if(sessionData.TryGetLatestServerTime(out var serverTime))
            {
                sessionData.TryGetWhenServerTime(out var realtimeSinceStartup);

                var secondsToAdd = Time.realtimeSinceStartup - realtimeSinceStartup;

                dateTime = serverTime.AddSeconds(secondsToAdd);

                return true;
            }

            return false;
        }

        private void GGetServerOffSetTime()
        {
            StartCoroutine(gamificationPlayerEndpoints.CoGetTime((result, dateTime) =>
            {
                if(result == UnityWebRequest.Result.Success)
                {
                    OnServerTime?.Invoke(dateTime);
                } else
                {
                    StartCoroutine(ActionAfterXSeconds(GGetServerOffSetTime, 5f));
                }
            }));
        }

        private bool GTryGetCurrentLanguage(out string language)
        {
            return sessionData.TryGetLatestLanguage(out language);
        }

        private bool GTryGetLatestMicroGamePayload(out MicroGamePayload payload)
        {
            return sessionData.TryGetLatestMicroGamePayload(out payload);
        }

        private bool GTryGetLatestFitnessContentIdentifier(out string identifier)
        {
            return sessionData.TryGetLatestFitnessContentIdentifier(out identifier);
        }

        private bool GTryGetLatestSubdomain(out string subdomain)
        {
            return sessionData.TryGetLatestSubdomain(out subdomain);
        }

        private void GUseMockServer()
        {
            sessionData = new SessionLogData();

            gamificationPlayerEndpoints = new GamificationPlayerEndpoints(gamificationPlayerMockConfig, sessionData);
        }

        private void GProcessExternalMessage(string jsonMessage)
        {
            var message = jsonMessage.FromJson<StandardDTO>();

            OnEvent?.Invoke(message.data.Type);

            if(message.data.Type == "moduleSessionStarted")
            {
                ModuleSessionStarted(jsonMessage);
            }

            if(message.data.Type == "pageView")
            {
                PageView(jsonMessage);
            }

            if (message.data.Type == "microGameOpened")
            {
                MicroGameOpened(jsonMessage);
            }

            if (message.data.Type == "fitnessContentOpened")
            {
                FitnessContentOpened(jsonMessage);
            }
        }

        private void ModuleSessionStarted(string jsonMessage)
        {
            var dto = jsonMessage.FromJson<ModuleSessionStartedDTO>();

            if(sessionData.TryGetLatestModuleSessionId(out Guid latestModuleSessionId))
            {
                if(latestModuleSessionId == Guid.Parse(dto.data.attributes.module_session_id))
                {
                    sessionData.AddToLog(dto.data);

                    return;
                }
            }

            sessionData.AddToLog(dto.data);

            sessionData.TryGetLatestModuleId(out Guid id);
            
            InvokeModuleStart(id);
        }

        private void FitnessContentOpened(string jsonMessage)
        {
            var dto = jsonMessage.FromJson<FitnessContentOpenedDTO>();

            if(sessionData.TryGetLatestModuleSessionId(out Guid latestModuleSessionId))
            {
                var isAlreadyStarted = sessionData.LogData.OfType<ModuleSessionStartedDTO.Data>()
                    .Where(m => Guid.Parse(m.attributes.module_session_id) == latestModuleSessionId)
                    .Count() > 1;

                if(isAlreadyStarted)
                {
                    sessionData.AddToLog(dto.data);

                    return;
                }
            }

            sessionData.AddToLog(dto.data);

            var moduleSessionStartedData = sessionData.LogData.OfType<ModuleSessionStartedDTO.Data>().Last();

            sessionData.TryGetLatestUserId(out var id);

            sessionData.AddToLog(new ProcessModuleSessionStartedDTOToLoggableData().Process(moduleSessionStartedData));
        
            OnFitnessContentOpened?.Invoke(dto.data.attributes.identifier);
        }

        private void MicroGameOpened(string jsonMessage)
        {
            var dto = jsonMessage.FromJson<MicroGameOpenedDTO>();

            if(sessionData.TryGetLatestModuleSessionId(out Guid latestModuleSessionId))
            {
                var isAlreadyStarted = sessionData.LogData.OfType<ModuleSessionStartedDTO.Data>()
                    .Where(m => Guid.Parse(m.attributes.module_session_id) == latestModuleSessionId)
                    .Count() > 1;

                if(isAlreadyStarted)
                {
                    sessionData.AddToLog(dto.data);

                    return;
                }
            }

            sessionData.AddToLog(dto.data);

            var JSONWebTokenPayload = JWTHelper.GetJSONWebTokenPayload(dto.data.attributes.module_data, gamificationPlayerEndpoints.EnviromentConfig.JSONWebTokenSecret);
            var webTokenPayload = JSONWebTokenPayload.FromJson<MicroGamePayload>();

            sessionData.AddToLog(webTokenPayload);

            var moduleSessionStartedData = sessionData.LogData.OfType<ModuleSessionStartedDTO.Data>().Last();

            sessionData.TryGetLatestUserId(out var id);

            sessionData.AddToLog(new ProcessModuleSessionStartedDTOToLoggableData().Process(moduleSessionStartedData));
        
            InvokeMicroGameOpened(webTokenPayload);          
        }

        private void PageView(string jsonMessage)
        {
            sessionData.TryGetLatestLanguage(out var previousLanguage);

            var dto = jsonMessage.FromJson<PageViewDTO>();

            sessionData.AddToLog(dto.data);

            if(isDeviceFlowActive && 
                sessionData.TryGetLatestOrganisationId(out _) && 
                sessionData.TryGetLatestUserId(out _))
            {
                StopAllCoroutines();

                gamificationPlayerEndpoints.CoGetLoginToken(GetLoginTokenResult);
            }

            OnPageView?.Invoke();

            if(sessionData.TryGetLatestLanguage(out var language))
            {
                if(previousLanguage != language)
                {
                    OnLanguageSet?.Invoke(language);
                }
            }

            if(!sessionData.TryGetLatestSubdomain(out _) && 
                sessionData.TryGetLatestOrganisationId(out _))
            {
                StartCoroutine(gamificationPlayerEndpoints.CoGetOrganisation());
            }
        }

        private bool GIsUserActive()
        {
            return sessionData.TryGetLatestUserId(out _) &&
                sessionData.TryGetLatestOrganisationId(out _);
        }

        private bool GTryGetActiveUserId(out Guid id)
        {
            if(GIsUserActive())
            {
                return sessionData.TryGetLatestUserId(out id);
            }

            id = default;

            return false;
        }

        private bool GIsModuleSessionActive()
        {
            return sessionData.TryGetLatestModuleSessionStarted(out _) && 
                !sessionData.TryGetLatestModuleSessionEnded(out _);
        }

        private bool GTryGetActiveModuleId(out Guid id)
        {
            if(GIsModuleSessionActive())
            {
                return sessionData.TryGetLatestModuleId(out id);
            }

            id = default;

            return false;
        }

        private void GEndLatestModuleSession(int score, bool isCompleted, Action onDone = null)
        {
            GTryGetServerTime(out DateTime now);

            StartCoroutine(gamificationPlayerEndpoints.CoEndModuleSession(now, score, isCompleted, (_) =>
            {
                onDone?.Invoke();
            }));
        }

        private void GStartDeviceFlow(StartDeviceFlowCallback onStart)
        {
            if(isDeviceFlowActive)
            {
                GStopDeviceFlow();
            }

            isDeviceFlowActive = true;

            StartCoroutine(gamificationPlayerEndpoints.CoAnnounceDeviceFlow((result, loginUrl) =>
            {
                if(result == UnityWebRequest.Result.Success)
                {
                    onStart?.Invoke(loginUrl);

                    StartCoroutine(ActionAfterXSeconds(CheckDeviceFlow, 4f));
                } else
                {
                    onStart?.Invoke(string.Empty);
                }
            }));
        }

        private void GStopDeviceFlow()
        {
            isDeviceFlowActive = false;

            StopAllCoroutines();
        }

        private void CheckDeviceFlow()
        {
            StartCoroutine(gamificationPlayerEndpoints.CoGetDeviceFlow((result, isValidated, userId) =>
            {
                if(result == UnityWebRequest.Result.Success && isValidated)
                {
                    StartCoroutine(gamificationPlayerEndpoints.CoGetOrganisation(GetOrganisationResult));
                    StartCoroutine(gamificationPlayerEndpoints.CoGetLoginToken(GetLoginTokenResult));
                } else
                {
                    StartCoroutine(ActionAfterXSeconds(CheckDeviceFlow, 4f));
                }
            }));
        }

        private void GetOrganisationResult(UnityWebRequest.Result result, GetOrganisationResponseDTO dto)
        {
            if(result == UnityWebRequest.Result.Success)
            {
                EndLoginFlowIfCorrect();
            } else
            {
                StartCoroutine(ActionAfterXSeconds(() =>
                {
                    StartCoroutine(gamificationPlayerEndpoints.CoGetOrganisation(GetOrganisationResult));
                }, 4f));
            }
        }

        private void GetLoginTokenResult(UnityWebRequest.Result result, string token)
        {
            if(result == UnityWebRequest.Result.Success)
            {
                EndLoginFlowIfCorrect();
            } else
            {
                StartCoroutine(ActionAfterXSeconds(() =>
                {
                    StartCoroutine(gamificationPlayerEndpoints.CoGetLoginToken(GetLoginTokenResult));
                }, 4f));
            }
        }

        private void EndLoginFlowIfCorrect()
        {
            if(sessionData.TryGetLatestSubdomain(out var subdomain) &&
                sessionData.TryGetLatestLoginToken(out var loginToken))
            {
                var redirectURL = string.Format("https://{0}.{1}login?otlToken={2}", subdomain, gamificationPlayerEndpoints.EnviromentConfig.Webpage, loginToken);

                isDeviceFlowActive = false;
                
                InvokeUserLoggedIn(redirectURL);
            }
        }

        protected void InvokeUserLoggedIn(string redirectURL)
        {
            OnUserLoggedIn?.Invoke(redirectURL);
        }

        protected void InvokeModuleStart(Guid moduleId)
        {
            OnModuleStart?.Invoke(moduleId);
        }

        protected void InvokeMicroGameOpened(MicroGamePayload microGame)
        {
            OnMicroGameOpened?.Invoke(microGame);
        }

        protected IEnumerator ActionAfterXSeconds(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);

            action?.Invoke();
        }
    }
}