﻿/* ManagedClient64.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace ManagedClient
{
    using Gbl;
    using Sockets;
    using Transactions;

    [Serializable]
    public class ManagedClient64
        : IDisposable
    {
        #region Constants

        /// <summary>
        /// Разделитель строк в пакете запроса к серверу.
        /// </summary>
        public const char QueryLineDelimiter = (char)0x0A;

        /// <summary>
        /// Разделитель строк в пакете ответа сервера.
        /// </summary>
        public const string ResponseLineDelimiter = "";

        /// <summary>
        /// 
        /// </summary>
        public const int MaxPostings = 32758;

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultHost = "127.0.0.1";

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultDatabase = "IBIS";

        /// <summary>
        /// 
        /// </summary>
        public const IrbisWorkstation DefaultWorkstation
            = IrbisWorkstation.Cataloger;

        /// <summary>
        /// 
        /// </summary>
        public const int DefaultPort = 6666;

        ///// <summary>
        ///// 
        ///// </summary>
        //public const string DefaultUsername = "1";

        ///// <summary>
        ///// 
        ///// </summary>
        //public const string DefaultPassword = "1";

        /// <summary>
        /// Количество попыток повторения команды по умолчанию.
        /// </summary>
        public const int DefaultRetryCount = 5;

        /// <summary>
        /// Таймаут получения ответа от сервера по умолчанию.
        /// </summary>
        public const int DefaultTimeout = 30000;

        #endregion

        #region Events

        public event EventHandler<IrbisCommadEventArgs> ErrorHandler;

        /// <summary>
        /// Вызывается, когда меняется состояние Busy;
        /// </summary>
        public event EventHandler BusyChanged;

        /// <summary>
        /// Вызывается перед уничтожением объекта.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Вызывается при отсутствии логина/пароля для входа на сервер.
        /// </summary>
        public event EventHandler<IrbisAuthenticationEventArgs> AuthenticationNeeded;

        /// <summary>
        /// Вызывается при смене базы данных.
        /// </summary>
        public event EventHandler<DatabaseChangedEventArgs> DatabaseChanged;

        /// <summary>
        /// Трансформация запроса перед отсылкой на сервер.
        /// </summary>
// ReSharper disable once EventNeverInvoked
        public event EventHandler<BeforeQueryEventArgs> BeforeQuery;

        /// <summary>
        /// Трансформация ответа после получения с сервера.
        /// </summary>
// ReSharper disable once EventNeverInvoked
        public event EventHandler<AfterQueryEventArgs> AfterQuery;

        /// <summary>
        /// Отлавливание транзакции по созданию, модификации или удалению записей.
        /// </summary>
        public event EventHandler<IrbisTransactionEventArgs> Transaction;

        #endregion

        #region Properties

        public static Version Version = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version;

        //[Browsable(false)]
        public bool Busy { get; private set; }

        /// <summary>
        /// Адрес сервера.
        /// </summary>
        /// <value>Адрес сервера в цифровом виде.</value>
        [DefaultValue(DefaultHost)]
        public string Host { get; set; }

        /// <summary>
        /// Порт сервера.
        /// </summary>
        /// <value>Порт сервера (по умолчанию 6666).</value>
        [DefaultValue(DefaultPort)]
        public int Port { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        /// <value>Имя пользователя.</value>
        //[DefaultValue(DefaultUsername)]
        public string Username { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        /// <value>Пароль пользователя.</value>
        //[DefaultValue(DefaultPassword)]
        public string Password { get; set; }

        /// <summary>
        /// Имя базы данных.
        /// </summary>
        /// <value>Служебное имя базы данных (например, "IBIS").</value>
        [DefaultValue(DefaultDatabase)]
        public string Database 
        { 
            get { return _database; }
            set 
            { 
                OnDatabaseChanged
                (
                    _database,
                    value
                );
                if (_irbisDatabases != null)
                    _irbisDatabases.FindDatabase();
            }
        }

        /// <summary>
        /// Тип АРМ.
        /// </summary>
        /// <value>По умолчанию <see cref="IrbisWorkstation.Cataloger"/>.
        /// </value>
        [DefaultValue(DefaultWorkstation)]
        public IrbisWorkstation Workstation { get; set; }

        /// <summary>
        /// Конфигурация клиента.
        /// </summary>
        /// <value>Высылается сервером при подключении.</value>
        public string Configuration
        {
            get { return _configuration; }
        }

        /// <summary>
        /// Статус подключения к серверу.
        /// </summary>
        /// <value>Устанавливается в true при успешном выполнении
        /// <see cref="Connect"/>, сбрасывается при выполнении
        /// <see cref="Disconnect"/> или <see cref="Dispose"/>.</value>
        public bool Connected
        {
            get { return _connected; }
        }

        /// <summary>
        /// Для ожидания окончания запроса.
        /// </summary>
        public WaitHandle WaitHandle
        {
            get { return _waitHandle; }
        }

        /// <summary>
        /// Поток для вывода отладочной информации.
        /// </summary>
        /// <remarks><para><c>null</c> означает, что вывод отладочной 
        /// информации не нужен.</para>
        /// <para>Обратите внимание, что <see cref="DebugWriter"/>
        /// не сериализуется, т. к. большинство потоков не умеют
        /// сериализоваться. Так что при восстановлении клиента
        /// вам придётся восстанавливать <see cref="DebugWriter"/>
        /// самостоятельно.</para>
        /// </remarks>
        [DefaultValue(null)]
        public TextWriter DebugWriter
        {
            get { return _debugWriter; }
            set { _debugWriter = value; }
        }

        /// <summary>
        /// Разрешение делать шестнадцатиричный дамп полученных от сервера пакетов.
        /// </summary>
        [DefaultValue(false)]
        public bool AllowHexadecimalDump { get; set; }

        /// <summary>
        /// Количество повторений команды при неудаче.
        /// </summary>
        [DefaultValue(DefaultRetryCount)]
        public int RetryCount { get; set; }

        /// <summary>
        /// Таймаут получения ответа от сервера в миллисекундах
        /// (для продвинутых функций).
        /// </summary>
        [DefaultValue(DefaultTimeout)]
        public int Timeout { get; set; }

        [DefaultValue(false)]
        public bool Interrupted { get; set; }

        public IrbisIniFile Settings
        {
            get
            {
                if (_settings == null)
                {
                    string config = Configuration ?? string.Empty;
                    _settings = IniFile.ParseText<IrbisIniFile>(config);
                }
                return _settings;
            }
        }

        /// <summary>
        /// Флаг, устанавливающий необходимость парсинга поискового запроса с выделением ключевых слов
        /// </summary>
        
        [DefaultValue(false)]
        public bool NeedParseRequest { get; set; }

        public IrbisOpt OptFileRecord;

        public IrbisDatabaseContext IrbisDatabases
        {
            get
            {
                if (_irbisDatabases == null)
                    _irbisDatabases = new IrbisDatabaseContext(this);
                return _irbisDatabases;
            }
        }
        
        /// <summary>
        /// Произвольные пользовательские данные
        /// </summary>
        [Browsable(false)]
        public object UserData { get; set; }

        /// <summary>
        /// Этап работы.
        /// </summary>
        public string StageOfWork { get; set; }

        /// <summary>
        /// Работа с сокетами.
        /// </summary>
        public IrbisSocket Socket
        {
            get { return _socket; }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        /// <remarks>
        /// Обратите внимание, деструктор не нужен!
        /// Он помешает сохранению состояния клиента
        /// при сериализации и последующему восстановлению,
        /// т. к. попытается закрыть уже установленное
        /// соединение. Восстановленная копия клиента
        /// ломанётся в закрытое соедиение, и выйдет облом.
        /// </remarks>
        public ManagedClient64()
        {
            _waitHandle = new ManualResetEvent(true);

            // По умолчанию создаем простой синхронный сокет.
            _socket = new IrbisSocket();

            Host = DefaultHost;
            Port = DefaultPort;
            Database = DefaultDatabase;
            //Username = DefaultUsername;
            //Password = DefaultPassword;
            Username = null;
            Password = null;
            Workstation = DefaultWorkstation;
            RetryCount = DefaultRetryCount;
        }

        #endregion

        #region Private members

        private string _configuration;
        private bool _connected;

        [NonSerialized]
        private readonly ManualResetEvent _waitHandle;

        [NonSerialized]
        private TextWriter _debugWriter;

        [NonSerialized]
        private TcpClient _client;

        private int _userID;
        private int _queryID;

        [NonSerialized]
        private IrbisIniFile _settings;

        private IrbisSearchEngine SearchEngine;

        private string _database;

        private IrbisDatabaseContext _irbisDatabases;

        private readonly Encoding _utf8 = new UTF8Encoding(false, false);
        private readonly Encoding _cp1251 = Encoding.GetEncoding(1251);

        private IrbisSocket _socket;

        private readonly Stack<string> _databaseStack
            = new Stack<string>();

        private bool _OkToConnect
            (
                int returnCode
            )
        {
            if (!string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(Password))
            {
                return true;
            }

            EventHandler<IrbisAuthenticationEventArgs> handler = AuthenticationNeeded;
            if (ReferenceEquals(handler, null))
            {
                return false;
            }

            IrbisAuthenticationEventArgs eventArgs = new IrbisAuthenticationEventArgs
            {
                ReturnCode = returnCode,
                Username = Username,
                Password = Password
            };
            handler(this, eventArgs);
            if (!eventArgs.OkToConnect)
            {
                return false;
            }
            Username = Username;
            Password = Password;

            return !string.IsNullOrEmpty(Username)
                   && !string.IsNullOrEmpty(Password);
        }

        private void _CheckReturnCode
            (
                ResponseHeader response,
                // ReSharper disable UnusedParameter.Local
                params int[] allowed
                // ReSharper restore UnusedParameter.Local
            )
        {
            if ((response.ReturnCode < 0)
                 && !allowed.Contains(response.ReturnCode))
            {
                throw new IrbisException(response.ReturnCode);
            }
        }

        private void _CheckConnected()
        {
            if (!Connected)
            {
                throw new IrbisException("Not connected");
            }
        }

        private void _CheckBusy()
        {
            if (Busy)
            {
                throw new IrbisException("Busy");
            }
        }

        private void _SetBusy
            (
                bool newState
            )
        {
            if (newState)
            {
                Interrupted = false;
            }

            if (newState != Busy)
            {
                if (newState)
                {
                    _waitHandle.Reset();
                }
                else
                {
                    _waitHandle.Set();
                }
                Busy = newState;
                EventHandler handler = BusyChanged;
                if (handler != null)
                {
                    handler
                        (
                            this, 
                            EventArgs.Empty
                        );
                }
            }
        }

        private void _DebugDump
            (
                string text
            )
        {
            if (DebugWriter != null)
            {
                DebugWriter.WriteLine(text);
            }
        }

        private QueryHeader _CreateQuery(char command)
        {
            QueryHeader result = new QueryHeader
                                     {
                                         Command = command,
                                         Workstation = (char)Workstation,
                                         Password = Password,
                                         UserName = Username,
                                         QueryID = ++_queryID,
                                         ClientID = _userID
                                     };
            return result;
        }

        private string _CombinePath
            (
                IrbisPath path,
                string fileName
            )
        {
            string result;

            switch (path)
            {
                case IrbisPath.System:
                case IrbisPath.Data:
                    result = string.Format
                        (
                            "{0}..{1}",
                            (int)path,
                            fileName
                        );
                    break;
                default:
                    result = string.Format
                        (
                            "{0}.{1}.{2}",
                            (int)path,
                            Database,
                            fileName
                        );
                    break;
            }

            return result;
        }

        private string _CombinePath
            (
                IrbisPath path,
                string database,
                string fileName
            )
        {
            string result;

            switch (path)
            {
                case IrbisPath.System:
                case IrbisPath.Data:
                    result = string.Format
                        (
                            "{0}..{1}",
                            (int)path,
                            fileName
                        );
                    break;
                default:
                    result = string.Format
                        (
                            "{0}.{1}.{2}",
                            (int)path,
                            database,
                            fileName
                        );
                    break;
            }

            return result;
        }

        private string _BoolToString
            (
                bool flag
            )
        {
            return flag ? "1" : "0";
        }

        private string _StripHash
            (
                string text
            )
        {
            int index = text.IndexOf('#');
            return (index >= 0)
                       ? text.Substring(index + 1)
                       : text;
        }

        private void _AddLine
            (
            StringBuilder builder,
            string line
            )
        {
            builder.Append(line);
            builder.Append(QueryLineDelimiter);
        }

        private string[] _Encode
            (
                IEnumerable<string> header,
                bool ansiData,
                IEnumerable<string> data
            )
        {
            StringBuilder result1 = new StringBuilder(),
                result2 = new StringBuilder();

            foreach (string line in header)
            {
                _AddLine(result1, line);
            }

            if (data != null)
            {
                foreach (string line in data)
                {
                    _AddLine(result2, line);
                }
            }

            if (result2.Length != 0)
            {
                char lastChar = result2[result2.Length - 1];
                if (lastChar == QueryLineDelimiter)
                {
                    result2.Length--;
                }
            }

            int packetLength = _cp1251.GetByteCount(result1.ToString())
                + (ansiData ? _cp1251 : _utf8).GetByteCount(result2.ToString());

            result1.Insert
                (
                    0,
                    packetLength.ToString(CultureInfo.InvariantCulture) + QueryLineDelimiter
                );

            return new[] { result1.ToString(), result2.ToString() };
        }

        private string[] _Encode
            (
                QueryHeader header,
                bool ansiData,
                params string[] data
            )
        {
            return _Encode(header.Encode(), ansiData, data);
        }

        private void _Send
            (
                QueryHeader header,
                bool ansiData,
                params string[] data
            )
        {
            string[] lines = _Encode(header, ansiData, data);
            _DebugDump(lines[0] + lines[1]);
            byte[] bytes1 = _cp1251.GetBytes(lines[0]);
            byte[] bytes2 = (ansiData ? _cp1251 : _utf8).GetBytes(lines[1]);
            byte[] bytes = new byte[bytes1.Length + bytes2.Length];
            Array.Copy(bytes1, 0, bytes, 0, bytes1.Length);
            Array.Copy(bytes2, 0, bytes, bytes1.Length, bytes2.Length);
            _client.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void _Send
            (
                QueryHeader header,
                params string[] data
            )
        {
            _Send(header, false, data);
        }

        private Encoding _Encoding
            (
                bool cp1251
            )
        {
            return cp1251 ? _cp1251 : _utf8;
        }

        private string _Receive
            (
                bool cp1251
            )
        {
            byte[] buffer = _client.GetStream().ReadToEnd();

            if (AllowHexadecimalDump
                 && (DebugWriter != null))
            {
                DebugWriter.WriteLine("Received:");
                Utilities.DumpBytes(DebugWriter, buffer, 0, buffer.Length);
            }

            string result = _Encoding(cp1251)
                .GetString
                (
                    buffer,
                    0,
                    buffer.Length
                );

            return result;
        }

        private void _OpenSocket()
        {
            if (_client != null)
            {
                throw new IrbisException("Socket already created");
            }

            IPAddress address;
            try
            {
                address = IPAddress.Parse(Host);
            }
            catch
            {
                IPAddress[] addresses = Dns.GetHostAddresses(Host);
                address = addresses[0];
            }
            _client = new TcpClient();
            _client.Connect(address, Port);
        }

        private void _CloseSocket()
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }

        private void OnDatabaseChanged
            (
                string oldDatabase,
                string newDatabase
            )
        {
            if (string.IsNullOrEmpty(newDatabase))
            {
                throw new ArgumentException("Bad database name");
            }

            _database = newDatabase;

            if (string.Compare
                (
                    oldDatabase,
                    newDatabase,
                    StringComparison.InvariantCultureIgnoreCase
                )
                != 0)
            {
                EventHandler<DatabaseChangedEventArgs> handler = DatabaseChanged;
                if (handler != null)
                {
                    DatabaseChangedEventArgs eventArgs = new DatabaseChangedEventArgs
                        (
                            oldDatabase,
                            newDatabase
                        );

                    handler
                        (
                            this, 
                            eventArgs
                        );
                }
            }
        }

        private IrbisTransactionItem OnTransaction
            (
                IrbisTransactionAction action,
                IrbisRecord record
            )
        {
            IrbisTransactionItem result = new IrbisTransactionItem
            {
                Moment = DateTime.Now,
                Action = action,
                Database = record.Database,
                Mfn = record.Mfn
            };

            var handler = Transaction;
            if (handler != null)
            {
                IrbisTransactionEventArgs eventArgs = new IrbisTransactionEventArgs
                {
                    Client = this,
                    Context = null,
                    Item = result
                };
                handler(this, eventArgs);
            }

            return result;
        }

        private bool OnBeforeQuery()
        {
            return true;
        }

        private void OnAfterQuery()
        {
            
        }

        #endregion

        #region Public methods

        public void ParseConnectionString
            (
                string connectionString
            )
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }
            connectionString = Regex.Replace
                (
                    connectionString,
                    @"\s+",
                    string.Empty
                );
            if (string.IsNullOrEmpty(connectionString)
                 || !connectionString.Contains("="))
            {
                throw new ArgumentException("connectionString");
            }

            Regex regex = new Regex
                (
                    "(?<name>[^=;]+?)=(?<value>[^;]+)",
                    RegexOptions.IgnoreCase
                    | RegexOptions.IgnorePatternWhitespace
                );
            MatchCollection matches = regex.Matches(connectionString);
            foreach (Match match in matches)
            {
                string name =
                    match.Groups["name"].Value.ToLower();
                string value = match.Groups["value"].Value;
                switch (name)
                {
                    case "host":
                    case "server":
                    case "address":
                        Host = value;
                        break;
                    case "port":
                        Port = int.Parse(value);
                        break;
                    case "user":
                    case "username":
                    case "name":
                    case "login":
                        Username = value;
                        break;
                    case "pwd":
                    case "password":
                        Password = value;
                        break;
                    case "db":
                    case "catalog":
                    case "database":
                        Database = value;
                        break;
                    case "arm":
                    case "workstation":
                        Workstation = (IrbisWorkstation)(byte)(value[0]);
                        break;
                    case "data":
                        UserData = value;
                        break;
                    case "debug":
                        StartDebug(value);
                        break;
                    case "etr":
                    case "stage":
                        StageOfWork = value;
                        break;
                    default:
                        throw new ArgumentException("connectionString");
                }
            }
        }

        /// <summary>
        /// Устанавливает подключение к новой базе.
        /// Запоминает, к какой базе был подключен
        /// клиент на момент смены.
        /// </summary>
        /// <param name="newDatabase">Новая база данных.</param>
        /// <returns>Предыдущая база данных.</returns>
        public string PushDatabase
            (
                string newDatabase
            )
        {
            string result = Database;
            _databaseStack.Push(Database);
            Database = newDatabase;
            return result;
        }

        /// <summary>
        /// Восстанавливает подключение к предыдущей
        /// базе данных.
        /// </summary>
        /// <returns>Имя базы данных, к которой
        /// был подключен клиент на момент восстановления
        /// состояния.</returns>
        public string PopDatabase()
        {
            string result = Database;
            Database = _databaseStack.Pop();
            return result;
        }

        public static string EncodeNewLines
            (
                string text
            )
        {
            string result = text.Replace
                (
                    "\r\n",
                    "\x001F\x001E"
                );
            return result;
        }

        public static string DecodeNewLines
            (
                string text
            )
        {
            string result = text.Replace
                (
                    "\x001F\x001E",
                    "\r\n"
                );
            return result;
        }

        public void Reconnect()
        {
            _connected = false;
            Connect();
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        // ReSharper disable once UnusedParameter.Local
        private object _Connect
            (
                object notUsed,
                IrbisCommadEventArgs eventArgs
            )
        {
            if (Connected)
            {
                return null;
            }

            _CheckBusy();
            int returnCode = 0;

TryAgain:
            if (!_OkToConnect(returnCode))
            {
                throw new ApplicationException("Can't connect");
            }

            _userID = new Random().Next(400000, 600000);
            _queryID = 0;            

            try
            {
                _SetBusy(true);
                _OpenSocket();

                eventArgs.QueryHeader = _CreateQuery('A');
                _Send
                    (
                        eventArgs.QueryHeader,
                        true,
                        Username,
                        Password
                    );
                string answer = _Receive(true);
                eventArgs.Response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _configuration = string.Join
                    (
                        Environment.NewLine,
                        eventArgs.Response.Data.ToArray(),
                        2,
                        eventArgs.Response.Data.Count - 2
                    );
                _settings = null;

                returnCode = eventArgs.Response.ReturnCode;
                if ((returnCode > -4000)
                    && (returnCode < -3000))
                {
                    // Коды: неверный пароль, нет доступа к АРМ и проч.
                    goto TryAgain;
                }

                _CheckReturnCode(eventArgs.Response);
                _connected = true;
            }
            catch (Exception exception)
            {
                string errorMessage = string.Format
                    (
                        "Network Error. Response={0}",
                        eventArgs.Response
                    );

                throw new IrbisException
                    (
                        errorMessage,
                        exception
                    );
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);                
            }
            return null;
        }

        public void Connect()
        {
            // Возвращаемое значение не используется.
            // Просто сигнатуры методов сделаны единообразными
            _Connect(null, new IrbisCommadEventArgs(this));
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        // ReSharper disable once UnusedParameter.Local
        private object _Disconnect
            (
                object notUsed,
                IrbisCommadEventArgs eventArgs
            )
        {
            if (!Connected)
            {
                return null;
            }

            _CheckBusy();

            try
            {
                _SetBusy(true);

                _OpenSocket();
                eventArgs.QueryHeader = _CreateQuery('B');
                _Send(eventArgs.QueryHeader, true, Username);
                string answer = _Receive(true);
                _DebugDump(answer);
                eventArgs.Response = ResponseHeader.Parse(answer);
                _CheckReturnCode(eventArgs.Response,-1);
                _settings = null;
                _connected = false;
            }
            catch (Exception exception)
            {
                throw new IrbisException("Inner exception", exception);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }

            return null;
        }

        public void Disconnect()
        {
            // Возвращаемое значение не используется.
            // Просто сигнатуры методов сделаны единообразными
            _Disconnect(null, new IrbisCommadEventArgs(this));
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        // ReSharper disable once UnusedParameter.Local
        private object _NoOp
            (
                object notUsed,
                IrbisCommadEventArgs eventArgs
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);

                _OpenSocket();

                eventArgs.QueryHeader = _CreateQuery('N');
                _Send(eventArgs.QueryHeader, Password, Username);
                string answer = _Receive(true);
                _DebugDump(answer);
                eventArgs.Response = ResponseHeader.Parse(answer);
                _CheckReturnCode(eventArgs.Response);
            }
            catch (Exception exception)
            {
                throw new IrbisException("Inner exception", exception);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);                
            }

            return null;
        }

        public void NoOp()
        {
            // Возвращаемое значение не используется.
            // Просто сигнатуры методов сделаны единообразными
            _NoOp(null, new IrbisCommadEventArgs(this));
        }

        public void WriteIni
            (
                string[] iniText
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();

                QueryHeader query = _CreateQuery('8');
                _Send
                    (
                        query,
                        iniText
                    );
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public IrbisVersion GetVersion()
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);

                _OpenSocket();

                QueryHeader query = _CreateQuery('1');
                _Send(query, null);
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                IrbisVersion result = IrbisVersion.Parse(response.Data);
                return result;
            }
            catch (Exception exception)
            {
                throw new IrbisException("Inner exception", exception);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string ReadTextFile
            (
                IrbisPath path,
                string fileName
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('L');
                string combinedPath = _CombinePath
                    (
                        path,
                        fileName
                    );
                _Send(query, combinedPath);
                string answer = _Receive(true);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response);
                string text = string.Join
                    (
                        Environment.NewLine,
                        response.Data.ToArray()
                    );
                text = DecodeNewLines(text).Trim();
                return text;
            }
            catch (Exception exception)
            {
                throw new IrbisException("Inner exception", exception);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string ReadTextFile
            (
                string name
            )
        {
            string result = ReadTextFile(IrbisPath.MasterFile, name);
            
            // Вся эта возня не нужна: ИРБИС сам отыскивает файлы.
            //if (string.IsNullOrEmpty(result) || (result == "\r\n"))
            //{
            //    result = ReadTextFile(IrbisPath.ParameterFile, name);
            //}

            return result;
        }

        public string[] ListFiles
            (
                IrbisPath path,
                string database,
                string mask
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('!');
                string combinedPath = _CombinePath
                    (
                        path,
                        database,
                        mask
                    );
                _Send(query, combinedPath);
                string answer = _Receive(true);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response);
                string text = string.Join
                    (
                        Environment.NewLine,
                        response.Data.ToArray()
                    );
                text = DecodeNewLines(text);
                return text.SplitLines();
            }
            catch (Exception exception)
            {
                throw new IrbisException("Inner exception", exception);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void WriteTextFile
            (
                IrbisPath path,
                string database,
                string fileName,
                string fileText
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('L');
                fileName = fileName + "&" + EncodeNewLines(fileText);
                string combinedPath = _CombinePath
                    (
                        path,
                        database,
                        fileName
                    );
                _Send(query, combinedPath);
                string answer = _Receive(true);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response);
            }
            catch (Exception exception)
            {
                throw new IrbisException("Inner exception", exception);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        private int _GetPosition
            (
                string markString, 
                byte[] buffer
            )
        {
            byte[] markBytes = _Encoding(true).GetBytes(markString);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == markBytes[0])
                {
                    bool match = true;
                    for (int j = 0; j < markBytes.Length; j++)
                    {
                        if (buffer[i + j] != markBytes[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private Stream _ReceiveBinary(out string result)
        {
            byte[] buffer = _client.GetStream().ReadToEnd();

            if (AllowHexadecimalDump
                 && (DebugWriter != null))
            {
                DebugWriter.WriteLine("Received:");
                Utilities.DumpBytes(DebugWriter, buffer, 0, buffer.Length);
            }

            MemoryStream stream = new MemoryStream(buffer, false);
            StreamReader reader = new StreamReader(stream, _Encoding(true));

            List<string> list = new List<string>();
            while (list.Count < 10 && !reader.EndOfStream)
                list.Add(reader.ReadLine());

            //вначала бинарного блока идет строка IRBIS_BINARY_DATA
            //прочитаем ее            
            char[] headingBuffer = new char[17];
            int read = reader.Read(headingBuffer, 0, headingBuffer.Length);
            string markString = new string(headingBuffer, 0, read);
            //если строки IRBIS_BINARY_DATA нет, вернуть все назад
            if (markString != "IRBIS_BINARY_DATA")
                throw new Exception("Сервер не передал массив бинарных данных");
            list.Add(markString);
            result = String.Join
                (
                    Environment.NewLine, 
                    list.ToArray ()
                );
            list.Clear();
            int position = _GetPosition(markString, buffer) 
                + _Encoding(true).GetByteCount(markString);

            MemoryStream ms = new MemoryStream(buffer, position, buffer.Length - position, false);
            return ms;

        }

        public Stream ReadBinaryFile
            (
                IrbisPath path,
                string fileName
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('L');
                string combinedPath = _CombinePath
                    (
                        path,
                        "@" + fileName
                    );
                _Send(query, combinedPath);
                string answer;
                Stream stream = _ReceiveBinary(out answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response);

                return stream;
            }
            catch (Exception exception)
            {
                throw new IrbisException("Inner exception", exception);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string[] ReadRawRecord
            (
                int mfn,
                bool needLock
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('C');
                _Send
                    (
                        query,
                        Database,
                        mfn.ToInvariantString(),
                        _BoolToString(needLock)
                    );
                string answer = _Receive(false);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response, -603); // Record can be logically deleted
                return response.Data.Skip(1).ToArray();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string[] ReadRawRecords
            (
                IEnumerable<int> mfns
            )
        {
            if (mfns == null)
            {
                throw new ArgumentNullException("mfns");
            }

            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                int[] array = mfns.ToArray();
                List<string> data = new List<string>
                                           {
                                               Database,
                                               "&unifor('+0')",
                                               array.Length.ToInvariantString ()
                                           };
                data.AddRange(array.Select(_ => _.ToInvariantString()));
                QueryHeader query = _CreateQuery('G');
                _Send
                    (
                        query,
                        data.ToArray()
                    );
                string answer = _Receive(false);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response);
                return response.Data
                               .Skip(1)
                               .Where(_ => !string.IsNullOrEmpty(_))
                               .ToArray();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        /// <summary>
        /// Чтение записи с последующим раскодированием по полям.
        /// </summary>
        /// <param name="mfn">MFN записи для чтения.</param>
        /// <returns>Раскодированная по полям запись.</returns>
        public IrbisRecord ReadRecord
            (
                int mfn
            )
        {
            string[] text = ReadRawRecord(mfn, false);
            IrbisRecord result = IrbisRecord.Parse(text,0);
            result.Database = Database;
            return result;
        }

        public IrbisRecord ReadRecordDatabase
            (
                string database,
                int mfn
            )
        {
            using ( new IrbisContextSaver (this) )
            {
                Database = database;
                return ReadRecord ( mfn );
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        // ReSharper disable once UnusedParameter.Local
        private int _GetMaxMfn
            (
                object notUsed,
                IrbisCommadEventArgs eventArgs
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                eventArgs.QueryHeader = _CreateQuery('O');
                _Send(eventArgs.QueryHeader, Database);
                string answer = _Receive(true);
                eventArgs.Response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(eventArgs.Response);
                return eventArgs.Response.ReturnCode;
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public int GetMaxMfn()
        {
            return _GetMaxMfn(null, new IrbisCommadEventArgs(this));
        }

        public int GetMaxMfnDatabase
            (
                string database
            )
        {
            using ( new IrbisContextSaver (this) )
            {
                Database = database;
                return GetMaxMfn ();
            }
        }

        public IrbisRecord[] ReadRecords
            (
                IEnumerable<int> mfns
            )
        {
            string[] lines = ReadRawRecords(mfns);
            IrbisRecord[] result = lines
                .Select(text => IrbisRecord.Parse(text,1))
                .ToArray();

            foreach (IrbisRecord record in result)
            {
                record.Database = Database;
            }

            return result;
        }

        public void WriteRecords
            (
                IrbisRecord[] records,
                bool needLock,
                bool ifUpdate
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('6');
                StringBuilder encoded = new StringBuilder();
                bool first = true;
                foreach (IrbisRecord record in records)
                {
                    if (!first)
                    {
                        encoded.Append("\x0D\x0A");
                    }
                    string database = record.Database ?? Database;
                    string one = database 
                        + "\x001E\x001F" 
                        + IrbisRecord.EncodeRecord
                        (
                            record,
                            record.Mfn,
                            (int)record.Status,
                            record.Version
                        );
                    encoded.Append(one);
                    first = false;
                }
                _Send
                    (
                        query,
                        _BoolToString(needLock),
                        _BoolToString(ifUpdate),
                        encoded.ToString()
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                for (int i = 0; i < records.Length; i++)
                {
                    IrbisRecord record = records[i];
                    IrbisTransactionAction action = (record.Mfn == 0)
                        ? IrbisTransactionAction.CreateRecord
                        : IrbisTransactionAction.ModifyRecord;
                    if (record.Database == null)
                    {
                        record.Database = Database;
                    }
                    record.Fields.Clear();
                    List<string> data = new List<string>
                        (
                            response.Data[i+1].Split('\x001E')
                        );
                    record.MergeParse
                        (
                            data.ToArray(),
                            0
                        );
                    OnTransaction
                        (
                            action,
                            record
                        );
                }
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void WriteRecord
            (
                IrbisRecord record,
                bool needLock,
                bool ifUpdate
            )
        {
            if ((record.Mfn == 0)
                && record.Deleted
                )
            {
                return;
            }

            _CheckConnected();
            _CheckBusy();

            using ( new IrbisContextSaver ( this ) )
            {
                if ( !string.IsNullOrEmpty ( record.Database ) )
                {
                    Database = record.Database;
                }

                try
                {
                    _SetBusy(true);
                    _OpenSocket ();
                    QueryHeader query = _CreateQuery ( 'D' );
                    string encoded = IrbisRecord.EncodeRecord
                        (
                            record,
                            record.Mfn,
                            (int) record.Status,
                            record.Version
                        );
                    _Send
                        (
                            query,
                            Database,
                            _BoolToString ( needLock ),
                            _BoolToString ( ifUpdate ),
                            encoded
                        );
                    string answer = _Receive ( false );
                    _DebugDump ( answer );
                    ResponseHeader response = ResponseHeader.Parse ( answer );
                    _CheckReturnCode ( response );
                    IrbisTransactionAction action = (record.Mfn == 0)
                        ? IrbisTransactionAction.CreateRecord 
                        : (
                            record.Deleted
                            ? IrbisTransactionAction.ModifyRecord
                            : IrbisTransactionAction.DeleteRecord
                          );
                    if (record.Database == null)
                    {
                        record.Database = Database;
                    }
                    record.Fields.Clear();
                    List<string> data = new List<string> {response.Data[1]};
                    data.AddRange(response.Data[2].Split('\x001E'));
                    record.MergeParse
                        (
                            data.ToArray(),
                            0
                        );
                    OnTransaction
                        (
                            action, 
                            record
                        );
                }
                finally
                {
                    _CloseSocket ();
                    _SetBusy(false);
                }
            }
        }

        public void WriteRecordDatabase
            (
                string database,
                IrbisRecord record,
                bool needLock,
                bool ifUpdate
            )
        {
            IrbisRecord clone = record.Clone ();
            clone.Database = database;

            WriteRecord ( clone, needLock, ifUpdate );
        }

        public string[] RawSearch
            (
                string expression,
                int offset,
                int count,
                string format,
                out int total
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('K');
                _Send
                    (
                        query,
                        Database,
                        expression,
                        count.ToInvariantString(),
                        offset.ToInvariantString(),
                        format
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                total = int.Parse(response.Data[1]);
                return response.Data.Skip(2).ToArray();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public int[] Search
            (
                string format,
                params object[] args
            )
        {
            string expression = string.Format
                (
                    format,
                    args
                );            

            List<int> result = new List<int>();

            int offset = 1;
            int count = 0;

            while (true)
            {
                int total;

                string[] found = RawSearch
                    (
                        expression,
                        offset,
                        count,
                        string.Empty,
                        out total
                    );

                int received = found.Length;

                int[] result0 = found
                    .Where(_ => !string.IsNullOrEmpty(_))
                    .Select(_ => int.Parse(_))
                    .ToArray();

                result.AddRange(result0);

                int rest = total - offset - received;
                if (rest <= 0)
                {
                    break;
                }

                offset += received;
                count = rest + 2;
            }

            try
            {
                if (NeedParseRequest)
                    IrbisDatabases.dataBases[IrbisDatabases.SelectedIndex].SearchEngine.ParseAddRequest(expression);
            }
            catch (RequestParsingException)
            {
            }

            return result
                .ToArray();
        }

        /// <summary>
        /// Ищет по указанной базе данных.
        /// По окончанию поиска восстанавливает
        /// текущий контекст.
        /// </summary>
        public int[] SearchDatabase
            (
                string database,
                string format,
                params object[] args
            )
        {
            using ( new IrbisContextSaver (this) )
            {
                Database = database;
                return Search ( format, args );
            }
        }

        public IrbisRecord[] SearchRead
            (
                string expression,
                params object[] args
            )
        {
            expression = string.Format(expression, args);            

            int total;

            string[] found = RawSearch
                (
                    expression,
                    1,
                    0,
                    "&uf('+0')",
                    out total
                );

            IrbisRecord[] result = found
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(text => IrbisRecord.Parse(text,1))
                .ToArray();

            return result;
        }

        public IrbisRecord[] SearchReadDatabase
            (
                string database,
                string expression,
                params object[] args
            )
        {
            using ( new IrbisContextSaver (this) )
            {
                Database = database;
                return SearchRead ( expression, args );
            }
        }

        public IrbisRecord SearchReadOneRecord
            (
                string expression,
                params object[] args
            )
        {
            IrbisRecord[] result = SearchRead(expression, args);
            return ((result == null) || (result.Length == 0))
                       ? null
                       : result[0];
        }

        public IrbisRecord SearchReadOneRecordFromDatabase
            (
                string database,
                string expression,
                params object[] args
            )
        {
            using ( new IrbisContextSaver (this) )
            {
                Database = database;
                return SearchReadOneRecord ( expression, args );
            }
        }

        public string[] SearchFormat
            (
                string expression,
                string format
            )
        {
            int total;

            string[] found = RawSearch
                (
                    expression,
                    1,
                    0,
                    format,
                    out total
                );

            string[] result = found
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(_ => _StripHash(_))
                .ToArray();

            return result;
        }

        public string[] SearchFormatDatabase
            (
                string database,
                string expression,
                string format
            )
        {
            using ( new IrbisContextSaver (this) )
            {
                Database = database;
                return SearchFormat ( expression, format );
            }
        }

        public string[] RawScanSearch
            (
                string expression,
                int offset,
                int count,
                string format,
                string sequence
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('K');
                _Send
                    (
                        query,
                        Database,
                        expression,
                        count.ToInvariantString(),
                        offset.ToInvariantString(),
                        format,
                        sequence
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                return response.Data.Skip(2).ToArray();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string FormatRecord
            (
                string format,
                IrbisRecord record
            )
        {
            if (format == "@" /* и версия сервера ниже той, когда разработчики наконец исправят ошибку */)
                format += IrbisDatabases.dataBases[IrbisDatabases.SelectedIndex].OptFileRecord.SelectOptFile(record);

            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('G');
                string encoded = IrbisRecord.EncodeRecord
                    (
                        record,
                        record.Mfn,
                        (int)record.Status,
                        record.Version
                    );
                _Send
                    (
                        query,
                        Database,
                        format,
                        "-2",
                        encoded
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                return response.Data.Skip(1).MergeLines();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string FormatRecord
            (
                string format,
                int mfn
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('G');
                _Send
                    (
                        query,
                        Database,
                        format,
                        1.ToInvariantString(),
                        mfn.ToInvariantString()
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                return response.Data.Skip(1).MergeLines();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string FormatRecordDatabase
            (
                string database,
                string format,
                int mfn
            )
        {
            using ( new IrbisContextSaver (this) )
            {
                Database = database;
                return FormatRecord ( format, mfn );
            }
        }

        public string[] FormatRecords
            (
                string format,
                params int[] mfnList
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('G');
                List<string> tail = new List<string>
                                           {
                                               Database,
                                               format,
                                               mfnList.Length.ToInvariantString ()
                                           };
                tail.AddRange(mfnList.Select(_ => _.ToInvariantString()));
                _Send
                    (
                        query,
                        tail.ToArray()
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                return response
                    .Data
                    .Skip(1)
                    .Select(_ => _StripHash(_))
                    .Select(_ => _.Replace('\x1F', '\n'))
                    .ToArray();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string FormatRecordWithHighlight(string format, IrbisRecord record, string startMarker, string endMarker)
        {
            IrbisSearchEngine SearchEngine = IrbisDatabases.dataBases[IrbisDatabases.SelectedIndex].SearchEngine;
            if (SearchEngine.CurrentRequest == null || !SearchEngine.ProcessTerms)
                return FormatRecord(format, record);

            IrbisRecord newRecord;
            RecordField newField;
            String fieldStr;

            foreach (ManagedClient.IrbisSearchEngine.SearchTerm term in SearchEngine.CurrentRequest.SearchStuff.SearchTerms)
            {
                if (!term.IsPresent)
                    continue;

                foreach (ManagedClient.IrbisSearchEngine.PostingsParams posting in term.Postings)
                    if ((record.Mfn == 0 || posting.Mfn.Contains(record.Mfn)) && posting.Text.Length >= SearchEngine.MinLKWLight)
                    {
                        newRecord = new IrbisRecord();
                        newRecord.Mfn = record.Mfn;

                        foreach (RecordField field in record.Fields)
                        {
                            fieldStr = SearchEngine.MarkEntries(field.ToText(), term.Trunc ? term.Name : posting.Text);
                            if (fieldStr != field.ToText())
                                newField = new RecordField(field.Tag, fieldStr);
                            else
                                newField = field;

                            newRecord.Fields.Add(newField);
                        }
                        if (record.ToString() != newRecord.ToString())
                            record = newRecord;
                    }
            }

            string desc = FormatRecord(format, record);
            desc = desc.Replace("{select}", startMarker).Replace("{/select}", endMarker);
            return desc;
        }

        public string FormatRecordWithHighlight(string format, int mfn, string startMarker, string endMarker)
        {
            IrbisRecord record = ReadRecord(mfn);
            return FormatRecordWithHighlight(format, record, startMarker, endMarker);            
        }

        /// <summary>
        /// Актуализирует запись.
        /// </summary>
        /// <param name="mfn">MFN записи. 0 означает все 
        /// неактуализированные записи в базе.</param>
        public void ActualizeRecord
            (
                int mfn
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('F');
                _Send
                    (
                        query,
                        Database,
                        mfn.ToString(CultureInfo.InvariantCulture)
                    );
                string answer = _Receive(false);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        /// <summary>
        /// Truncates the database.
        /// </summary>
        /// <remarks><see cref="Workstation"/> must be set to
        /// <see cref="IrbisWorkstation.Administrator"/>.
        /// <see cref="Workstation"/> can't be switched at the
        /// runtime!
        /// </remarks>
        public void TruncateDatabase ()
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('S');
                _Send(query, Database);
                string answer = _Receive(false);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void TruncateDatabase
            (
                string database
            )
        {
            using ( new IrbisContextSaver ( this ) )
            {
                Database = database;
                TruncateDatabase();
            }
        }

        public void LockRecords
            (
                params int[] mfnList
            )
        {
            foreach (int mfn in mfnList)
            {
                ReadRawRecord(mfn, true);
            }
        }

        public void UnlockRecords
            (
                params int[] mfnList
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('Q');
                List<string> tail = new List<string> { Database };
                tail.AddRange(mfnList
                                    .Select(_ => _.ToString(CultureInfo.InvariantCulture)));
                _Send(query, tail.ToArray());
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        /// <summary>
        /// Получение информации о базе данных.
        /// </summary>
        /// <returns>список логически удаленных, физически удаленных, 
        /// неактуализированных и заблокированных записей.</returns>
        public IrbisDatabaseInfo GetDatabaseInfo ()
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('0');
                _Send(query, Database);
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                IrbisDatabaseInfo result
                    = IrbisDatabaseInfo.ParseServerResponse(response.Data.ToArray());
                result.Name = Database;
                return result;
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public IrbisDatabaseInfo GetDatabaseInfo
            (
                string database
            )
        {
            using ( new IrbisContextSaver (this) )
            {
                Database = database;
                return GetDatabaseInfo ();
            }
        }

        public IrbisDatabaseInfo[] ListDatabases
            (
                string listFile
            )
        {
            string menuFile = ReadTextFile(IrbisPath.Data, listFile);
            string[] lines = menuFile.SplitLines();
            IrbisDatabaseInfo[] result = IrbisDatabaseInfo.ParseMenu(lines);
            return result;
        }

        public IrbisDatabaseInfo[] ListDatabases()
        {
            return ListDatabases("dbnam1.mnu");
        }

        public IrbisServerStat GetServerStat()
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('+');
                query.Subcommand = '1';
                _Send(query, Database);
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                IrbisServerStat result = IrbisServerStat.Parse(response.Data.ToArray());
                return result;
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public SearchTermInfo[] GetSearchTerms
            (
                string startTerm,
                int count
            )
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('H');
                _Send
                    (
                        query,
                        Database,
                        startTerm,
                        count.ToInvariantString()
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response, -202, -203, -204);
                return SearchTermInfo.Parse(response.Data);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public SearchPostingInfo[] GetSearchTerms
        (
            string startTerm,
            int count,
            string format
        )
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException("format");
            }

            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('H');
                _Send
                    (
                        query,
                        Database,
                        startTerm,
                        count.ToInvariantString(),
                        format
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response, -202, -203, -204);
                return SearchPostingInfo.ParseFormatted(response.Data);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public SearchPostingInfo[] GetSearchTermsReverse
        (
            string startTerm,
            int count,
            string format
        )
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException("format");
            }

            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('P');
                _Send
                    (
                        query,
                        Database,
                        startTerm,
                        count.ToInvariantString(),
                        format
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response, -202, -203, -204);
                return SearchPostingInfo.ParseFormatted(response.Data);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public SearchTermInfo[] GetSearchTermsReverse
            (
                string startTerm,
                int count
            )
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('P');
                _Send
                    (
                        query,
                        Database,
                        startTerm,
                        count.ToInvariantString()
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response, -202, -203, -204);
                return SearchTermInfo.Parse(response.Data);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public int GetSearchPostingsCount
            (
                string startTerm
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('I');
                _Send
                    (
                        query,
                        Database,
                        "0",
                        "0",
                        string.Empty,
                        startTerm
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response, -202);
                SearchPostingInfo[] postings = SearchPostingInfo.Parse(response.Data);
                return (postings.Length == 0)
                    ? 0
                    : postings[0].Mfn;
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string[] GetSearchEntries
            (
                string prefix
            )
        {
            if (ReferenceEquals(prefix, null))
            {
                throw new ArgumentNullException("prefix");
            }

            prefix = prefix.ToUpperInvariant();

            List<string> result = new List<string>();

            string current = prefix;
            int prefixLength = prefix.Length;
            bool canContinue = true;

            do
            {
                SearchTermInfo[] terms = GetSearchTerms
                    (
                        current,
                        100
                    );
                foreach (SearchTermInfo term in terms)
                {
                    if (term.Text.StartsWith(prefix))
                    {
                        string text = term.Text.Substring
                            (
                                prefixLength, 
                                term.Text.Length - prefixLength
                            );
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (!result.Contains(text)
                                && (term.Count != 0))
                            {
                                result.Add(text);
                            }
                        }
                        current = prefix + text;
                    }
                    else
                    {
                        canContinue = false;
                        break;
                    }
                }
            } while (canContinue);

            return result.ToArray();
        }

        public SearchPostingInfo[] GetSearchPostings
            (
                string startTerm,
                int count,
                int first
            )
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (first < 0)
            {
                throw new ArgumentOutOfRangeException("first");
            }

            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('I');
                _Send
                    (
                        query,
                        Database,
                        count.ToInvariantString(),
                        first.ToInvariantString(),
                        string.Empty,
                        startTerm
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response, -202);
                return SearchPostingInfo.Parse(response.Data);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        /// <summary>
        /// Общее число обращений к серверу, выполненных данным клиентом.
        /// </summary>
        /// <returns>Число обращений к серверу.</returns>
        public int GetQueryCount()
        {
            return _queryID;
        }

        public void DeleteRecord
            (
                IrbisRecord record,
                bool ifUpdate
            )
        {
            record.Deleted = true;
            WriteRecord(record, false, ifUpdate);
        }

        public void DeleteRecords
            (
                IEnumerable<int> mfns,
                bool ifUpdate
            )
        {
            foreach (int mfn in mfns)
            {
                IrbisRecord record = ReadRecord(mfn);
                DeleteRecord(record, ifUpdate);
            }
        }

        public void UndeleteRecord
            (
                IrbisRecord record,
                bool ifUpdate
            )
        {
            record.Deleted = false;
            WriteRecord(record, false, ifUpdate);
        }

        public void CreateDatabase
            (
                string databaseName,
                string description,
                bool readerAccess
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('T');
                _Send
                    (
                        query,
                        databaseName,
                        description,
                        _BoolToString(readerAccess)
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void DeleteDatabase
            (
                string databaseName
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('W');
                _Send
                    (
                        query,
                        databaseName
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void RestartServer()
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('+');
                query.Subcommand = '8';
                _Send
                    (
                        query
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public IrbisProcessInfo[] GetProcessList()
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('+');
                query.Subcommand = '3';
                _Send
                    (
                        query
                    );
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                return IrbisProcessInfo.Parse(response.Data.ToArray());
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public IrbisUserInfo[] GetUserList()
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('+');
                query.Subcommand = '9';
                _Send
                    (
                        query
                    );
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                return IrbisUserInfo.Parse(response.Data.ToArray());
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void SetUserList
            (
                IrbisUserInfo[] users
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('+');
                string[] text = users
                    .Select(_ => _.Encode())
                    .ToArray();
                query.Subcommand = '7';
                _Send
                    (
                        query,
                        text
                    );
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void UnlockDatabase
            (
                string databaseName
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('U');
                _Send
                    (
                        query,
                        databaseName
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public bool IsDatabaseLocked
            (
                string databaseName
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                //QueryHeader query = _CreateQuery('#');
                QueryHeader query = _CreateQuery('0');
                _Send
                    (
                        query,
                        databaseName
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                if ( response.Data.Count > 6 )
                {
                    bool result = (Convert.ToInt32 ( response.Data[0] ) == 0)
                        && ( Convert.ToInt32 ( response.Data [ 6 ] ) != 0 );
                    return result;
                }
                return false;
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void RebuildDictionary
            (
                string database
            )
        {
            _CheckConnected();
            _CheckBusy();
            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('Z');
                _Send
                    (
                        query,
                        database
                    );
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void ReorganizeMasterFile
            (
                string database
            )
        {
            _CheckConnected();
            _CheckBusy();
            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('X');
                _Send
                    (
                        query,
                        database
                    );
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void ReorganizeDictionary
            (
                string database
            )
        {
            _CheckConnected();
            _CheckBusy();
            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('Y');
                _Send
                    (
                        query,
                        database
                    );
                string answer = _Receive(true);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public void ActualizeDatabase
            (
                string database
            )
        {
            try
            {
                PushDatabase(database);
                ActualizeRecord(0);
            }
            finally
            {
                PopDatabase();
            }
        }

        public void ExecuteGlobal
            (
                string searchExpression,
                int firstMfn,
                int howMany
            )
        {
            throw new NotImplementedException();
        }

        public IrbisRecord ExecuteFst
            (
                IrbisRecord record,
                string[] fstText
            )
        {
            throw new NotImplementedException();
        }

        public IrbisRecord CopyRecord
            (
                IrbisRecord record,
                string database,
                bool needLock,
                bool ifUpdate
            )
        {
            IrbisRecord result = record.Clone();
            result.Database = database;
            WriteRecord(result, needLock, ifUpdate);

            return result;
        }

        public IrbisRecord MoveRecord
            (
                IrbisRecord record,
                string database,
                bool needLock,
                bool ifUpdate
            )
        {
            IrbisRecord result = record.Clone();
            result.Database = database;
            WriteRecord(result, needLock, ifUpdate);
            DeleteRecord(record, ifUpdate);

            return result;
        }

        public string[] GetRawRecordHistory
            (
                int mfn,
                bool leaveLock,
                int entry
            )
        {
            _CheckConnected();
            _CheckBusy();

            bool needUnlock = false;

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('C');
                _Send
                    (
                        query,
                        Database,
                        mfn.ToInvariantString(),
                        entry.ToInvariantString()
                    );
                string answer = _Receive(false);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response, -201, -603);
                if (response.ReturnCode == -201)
                {
                    needUnlock = true;
                    return null;
                }
                return response.Data.Skip(1).ToArray();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);

                if (needUnlock)
                {
                    UnlockRecords(new[] { mfn });
                }
            }
        }

        public string[] GetRawRecordHistory
            (
                int mfn,
                int entry,
                bool leaveLock,
                string format
            )
        {
            _CheckConnected();
            _CheckBusy();

            bool needUnlock = false;

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('C');
                _Send
                    (
                        query,
                        Database,
                        mfn.ToInvariantString(),
                        entry.ToInvariantString(),
                        format
                    );
                string answer = _Receive(false);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _DebugDump(answer);
                _CheckReturnCode(response, -201, -603);
                if (response.ReturnCode == -201)
                {
                    needUnlock = true;
                    return null;
                }
                return response.Data.Skip(1).ToArray();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);

                if (!leaveLock && needUnlock)
                {
                    UnlockRecords(new[] { mfn });
                }
            }
        }

        public IrbisRecord GetRecordHistory
            (
                int mfn,
                bool leaveLock,
                int entry
            )
        {
            string[] text = GetRawRecordHistory(mfn, leaveLock, entry);
            if (text == null)
            {
                return null;
            }
            IrbisRecord result = IrbisRecord.Parse(text,0);
            if (result != null)
            {
                result.Database = Database;
            }
            return result;
        }

        public IrbisRecord[] GetRecordHistory
            (
                int mfn,
                bool leaveLock
            )
        {
            List<IrbisRecord> result = new List<IrbisRecord>();

            for (int entry = 1; entry < int.MaxValue; entry++)
            {
                IrbisRecord record = GetRecordHistory
                    (
                        mfn,
                        leaveLock,
                        entry
                    );
                if (record == null)
                {
                    break;
                }
                result.Add(record);
            }

            return result.ToArray();
        }

        public bool ForEachRecord
            (
                IEnumerable<int> mfns,
                Func<ManagedClient64, IrbisRecord, bool> func,
                int delta
            )
        {
            // ReSharper disable PossibleMultipleEnumeration
            if (!(mfns is int[]))
            {
                mfns = mfns.ToArray();
            }

            int offset = 0;

            while (true)
            {
                int[] portion = mfns
                    .Skip(offset)
                    .Take(delta)
                    .ToArray();

                if (portion.Length == 0)
                {
                    break;
                }

                IrbisRecord[] records = ReadRecords(portion);

                if (records.Any(record => !func(this, record)))
                {
                    return false;
                }

                offset += delta;
            }

            return true;
            // ReSharper restore PossibleMultipleEnumeration
        }

        public bool ForEachRecord
            (
                Func<ManagedClient64, IrbisRecord, bool> func,
                int delta
            )
        {
            IEnumerable<int> mfns = Enumerable.Range(1, GetMaxMfn() - 1);

            return ForEachRecord
                (
                    mfns,
                    func,
                    delta
                );
        }

        /// <summary>
        /// Глобальная корректировка
        /// </summary>
        /// <param name="searchExpression">Поисковое выражение, предварительно
        /// отбирающее записи.</param>
        /// <param name="searchFrom">Первый MFN в БД для поиска.</param>
        /// <param name="searchTo">Последний MFN в БД для поиска.</param>
        /// <param name="firstMfn">Первый mfn в результате поиска 
        /// для корректировки.</param>
        /// <param name="lastMfn">Последний mfn в результате поиска для корректировки.</param>
        /// <param name="mfns">Прямое перечисление MFN.</param>
        /// <param name="autoin">Автоввод</param>
        /// <param name="adjustments">Выражения для GBL</param>
        /// <param name="updateIf">Актуализировать словарь</param>
        /// <param name="flc">Формально-логический контроль</param>
        public GblResult[] GlobalAdjustment
            (
                string searchExpression,
                int searchFrom,
                int searchTo,
                int firstMfn,
                int lastMfn,
                int[] mfns,
                bool updateIf,
                bool flc,
                bool autoin,
                GblItem[] adjustments
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('5');
                List<string> data = new List<string>
                {
                    Database, 
                    _BoolToString(updateIf)
                };
                StringBuilder builder = new StringBuilder();
                builder.Append("!0");
                builder.Append(GblItem.Delimiter);
                foreach (GblItem item in adjustments)
                {
                    builder.Append(item);
                }
                builder.Append(GblItem.Delimiter);
                data.Add(builder.ToString());
                if (searchExpression == null)
                {
                    searchExpression = string.Empty;
                }
                data.Add(searchExpression);
                data.Add(searchFrom.ToInvariantString());
                data.Add(searchTo.ToInvariantString());
                
                data.Add(string.Empty); // Пустая строка неизвестного назначения

                if (mfns == null)
                {
                    data.Add(0.ToInvariantString());
                    data.Add(firstMfn.ToInvariantString());
                    data.Add(lastMfn.ToInvariantString());
                }
                else
                {
                    data.Add(mfns.Length.ToInvariantString());
                    foreach (int mfn in mfns)
                    {
                        data.Add(mfn.ToString("D7"));
                    }
                }
                if (!flc)
                {
                    data.Add("*"); // ???
                }
                if (!autoin)
                {
                    data.Add("&"); // ???
                }

                _Send
                    (
                        query,
                        data.ToArray()
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                return GblResult.Parse(response.Data.Skip(1));
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        /// <summary>
        /// Последовательный поиск.
        /// </summary>
        /// <returns></returns>
        public string[] SequentialSearch
            (
                string dictionaryExpression,
                int count,
                int offset,
                string format,
                int minMfn,
                int maxMfn,
                string booleanExpression
            )
        {
            _CheckConnected();
            _CheckBusy();

            try
            {
                _SetBusy(true);
                _OpenSocket();
                QueryHeader query = _CreateQuery('K');
                _Send
                    (
                        query,
                        Database,
                        dictionaryExpression,
                        count.ToInvariantString(),
                        offset.ToInvariantString(),
                        format
                    );
                string answer = _Receive(false);
                _DebugDump(answer);
                ResponseHeader response = ResponseHeader.Parse(answer);
                _CheckReturnCode(response);
                //total = int.Parse(response.Data[1]);
                return response.Data.Skip(2).ToArray();
            }
            finally
            {
                _CloseSocket();
                _SetBusy(false);
            }
        }

        public string[] SequentialSearchFormat
            (
                string dictionaryExpression,
                string format,
                string booleanExpression
            )
        {
            string[] found = SequentialSearch
                (
                    dictionaryExpression,
                    0,
                    1,
                    format,
                    0,
                    0,
                    booleanExpression
                );
            return found
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(_ => _StripHash(_))
                .ToArray();
        }

        public IrbisRecord[] SequentialSearchRead
            (
                string dictionaryExpression,
                string booleanExpression
            )
        {
            string[] found = SequentialSearch
                (
                    dictionaryExpression,
                    0,
                    1,
                    string.Empty,
                    0,
                    0,
                    booleanExpression
                );

            return found
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(text => IrbisRecord.Parse(text, 1))
                .ToArray();
        }

        public IrbisOpt GetOptInfo(String OptFileName)
        {
            int formatItemsCount;
            int index;
            String txtLine;
            try
            {
                string answer = ReadTextFile(IrbisPath.Data, Database + "\\" + OptFileName);
                IrbisOpt irbisOpt = new IrbisOpt();
                String[] OptBuffer = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); //answer.SplitLines()
                if (OptBuffer.Length == 0)
                {
                    return new IrbisOpt();
                }
                irbisOpt.OptFormatInfo.fieldId = Convert.ToInt32(OptBuffer[0]);
                irbisOpt.OptFormatInfo.docTypeMaxLen = Convert.ToInt32(OptBuffer[1]);
                formatItemsCount = OptBuffer.Length - 3;
                irbisOpt.OptFormatInfo.formatItems = new FormatItems[formatItemsCount];
                for (int i = 0; i < formatItemsCount; i++)
                {
                    txtLine = OptBuffer[i + 2];
                    index = txtLine.IndexOf(' ');
                    if (index == -1)
                        break;
                    irbisOpt.OptFormatInfo.formatItems[i].docType = txtLine.Substring(0, index);
                    while (txtLine[index] == ' ') index++;
                    irbisOpt.OptFormatInfo.formatItems[i].pftFilename = txtLine.Substring(index);
                }
                return irbisOpt;

            }
            catch (IrbisException)
            {
                return null;
            }
        }

        public void StartDebug(string fileName)
        {
            StopDebug();
            DebugWriter = new StreamWriter(fileName, true);
        }

        public void StopDebug()
        {
            if (DebugWriter != null)
            {
                DebugWriter.Close();
                DebugWriter = null;
            }
        }

        #endregion

        #region Object members

        #endregion

        #region Advanced functions

        public bool RequireServerVersion
            (
                string minimumVersion,
                bool throwException
            )
        {
            IrbisVersion actualVersion = GetVersion();
            bool result = string.CompareOrdinal
                (
                    actualVersion.Version,
                    ("64." + minimumVersion)
                ) >= 0;

            if (!result
                 && throwException)
            {
                string message = string.Format
                    (
                        "Required server version {0}, found version {1}",
                        minimumVersion,
                        actualVersion.Version
                    );
                throw new IrbisException(message);
            }

            return result;
        }

        public bool RequireClientVersion
            (
                Version minimumVersion,
                bool throwException
            )
        {
            bool result = Version.CompareTo(minimumVersion) >= 0;

            if (!result
                 && throwException)
            {
                string message = string.Format
                    (
                        "Required client version {0}, found version {1}",
                        minimumVersion,
                        Version
                    );
                throw new IrbisException(message);
            }

            return result;
        }

        private bool _Reconnect()
        {
            return false;
        }

        /// <summary>
        /// Данный метод нужен, чтобы клиент 
        /// не пытался самостоятельно закрыть соединение с сервером.
        /// Этот метод может пригодиться при сохранении состояния
        /// клиента с последующим восстановлением.
        /// </summary>
        public void Shutdown()
        {
            _connected = false;
        }

        /// <summary>
        /// Фиксирует в серверном INI-файле ФИО оператора и этап работы.
        /// Это нужно для работы &amp;unifor('IPRIVATE,FIO') 
        /// и &amp;unifor('IPRIVATE,ETR')
        /// </summary>
        public bool ReportSettingsToServer()
        {
            bool result = false;
            List<string> settings = new List<string>();

            settings.Add("[PRIVATE]");

            if (!string.IsNullOrEmpty(Username))
            {
                settings.Add(string.Concat("FIO=",Username));
                result = true;
            }
            if (!string.IsNullOrEmpty(StageOfWork))
            {
                settings.Add(string.Concat("ETR=",StageOfWork));
                result = true;
            }

            if (result)
            {
                WriteIni(settings.ToArray());
            }

            return result;
        }

        public static string SerializeToString
            (
                ManagedClient64 client
            )
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, client);
            byte[] array = stream.ToArray();
            string result = Convert.ToBase64String(array);
            return result;
        }

        public static ManagedClient64 DeserializeFromString
            (
                string text
            )
        {
            byte[] array = Convert.FromBase64String(text);
            MemoryStream stream = new MemoryStream(array);
            BinaryFormatter formatter = new BinaryFormatter();
            ManagedClient64 result = (ManagedClient64)formatter.Deserialize(stream);
            return result;
        }

        public bool CanFixThisError
            (
                IrbisCommadEventArgs eventArgs
            )
        {
            if (ReferenceEquals(eventArgs.Exception, null))
            {
                return true;
            }
            return false;
        }

        public void InvokeErrorHandler(IrbisCommadEventArgs eventArgs)
        {
            EventHandler<IrbisCommadEventArgs> handler = ErrorHandler;
            if (handler != null)
            {
                handler.Invoke(this, eventArgs);
            }
        }

        public TResult ExecuteFunction<TArgument, TResult>
            (
                Func<TArgument, IrbisCommadEventArgs, TResult> function,
                TArgument argument
            )
        {
            IrbisCommadEventArgs eventArgs
                = new IrbisCommadEventArgs(this);

            TResult result = default(TResult);
            while (eventArgs.RetryCount > 0)
            {
                try
                {
                    eventArgs.Exception = null;
                    result = function(argument, eventArgs);
                }
                catch (IrbisException ex)
                {
                    eventArgs.Exception = ex;
                }

                if (eventArgs.Exception == null)
                {
                    break;
                }
                {
                    InvokeErrorHandler(eventArgs);
                    if (eventArgs.StopExecution)
                    {
                        break;
                    }
                    if (!CanFixThisError(eventArgs))
                    {
                        break;
                    }
                    eventArgs.RetryCount--;
                }
            }

            return result;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (Connected)
            {
                Disconnect();
            }

            _CloseSocket();

            EventHandler handler = Disposing;
            if (!ReferenceEquals(handler, null))
            {
                try
                {
                    handler(this, EventArgs.Empty);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // Nothing to do here
                }
            }
        }

        #endregion

        #region Nested classes

        public class IrbisDatabaseContext
        {
            private ManagedClient64 client;
            public IrbisDatabase[] dataBases;
            public int SelectedIndex;

            public IrbisDatabaseContext(ManagedClient64 client)
            {
                this.client = client;

                if (!client._connected)
                    return;

                IniFile iniFile = IniFile.ParseText<IniFile>(client._configuration);
                string databaseListFilename = iniFile.Get<String>("MAIN", "DBNNAMECAT", "DBNAM1.MNU");

                try
                {
                    string answer = client.ReadTextFile(IrbisPath.Data, databaseListFilename);
                    string[] dbBuffer = answer.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int count = dbBuffer.Length / 2;
                    dataBases = new IrbisDatabase[count];
                    for (int i = 0; i < count; i++)
                        dataBases[i] = new IrbisDatabase(dbBuffer[i * 2], dbBuffer[i * 2 + 1], client);

                    FindDatabase();
                }
                catch
                {
                }
            }

            public void FindDatabase()
            {
                try
                {
                    IrbisDatabase database = dataBases.First(x => x.dbName == client.Database);
                    if (database != null)
                        SelectedIndex = Array.IndexOf<IrbisDatabase>(dataBases, database);
                }
                catch
                {
                }
            }

            public IrbisDatabase this[int index]
            {
                get
                {
                    return dataBases[index];
                }
            }

            public IrbisDatabase this[string name]
            {
                get
                {
                    return dataBases.First(x => x.dbName == name);
                }
            }
        }

        public class IrbisDatabase
        {
            public string dbName;
            public string dbDesc;
            public int[] mfn;
            public int SelectedMfn;
            private Dictionary<int, IrbisRecord> _records;
            private int _length;
            public int Length
            {
                get
                {
                    if (_length == 0 && client._connected)
                        _length = client.GetMaxMfn();
                    return _length;
                }
            }

            private ManagedClient64 client;


            private IrbisOpt _optFileRecord;

            public IrbisOpt OptFileRecord
            {
                get
                {
                    if (_optFileRecord == null)
                        _optFileRecord = GetOptInfo(PftOptFile);

                    return _optFileRecord;
                }
                set
                {
                    _optFileRecord = value;
                }
            }

            public IrbisRecord this[int mfn]
            {
                get
                {
                    if (mfn > 0 && mfn <= Length)
                    {
                        if (!_records.ContainsKey(mfn))
                            if (mfn < Length)
                                _records[mfn] = client.ReadRecord(mfn);
                            else
                                _records[mfn] = new IrbisRecord();

                        return _records[mfn];
                    }
                    else
                        return null;
                }
                set
                {
                    if (mfn > 0 && mfn <= Length)
                    {
                        _records[mfn] = value;
                        if (mfn < Length && _records[mfn].Mfn != mfn)
                            _records[mfn].Mfn = mfn;
                    }
                }
            }

            public void ReloadContext()
            {
                if (mfn.Length > 0)
                    mfn = new int[0];
                if (_records.Count > 0)
                    _records.Clear();
                _length = client.GetMaxMfn();
                if (_searchEngine != null)
                    _searchEngine = null;
            }

            public void Update()
            {
                if (SelectedMfn < 1 || SelectedMfn > Length || !_records.ContainsKey(SelectedMfn))
                    return;

                client.WriteRecord(_records[SelectedMfn], false, true);

                if (SelectedMfn == Length)
                    _length = client.GetMaxMfn();
            }

            public void RecordRollBack()
            {
                if (SelectedMfn < 1 || SelectedMfn > Length || !_records.ContainsKey(SelectedMfn))
                    return;

                if (SelectedMfn < Length)
                    _records[SelectedMfn] = client.ReadRecord(SelectedMfn);
                else
                    _records[SelectedMfn] = new IrbisRecord();
            }

            public void RecordClear()
            {
                if (SelectedMfn < 1 || SelectedMfn > Length || !_records.ContainsKey(SelectedMfn))
                    return;

                _records[SelectedMfn] = new IrbisRecord();
            }

            public IrbisOpt GetOptInfo(String OptFileName)
            {
                int formatItemsCount;
                int index;
                String txtLine;
                try
                {
                    string answer = client.ReadTextFile(IrbisPath.Data, this.dbName + "\\" + OptFileName);
                    IrbisOpt irbisOpt = new IrbisOpt();
                    String[] OptBuffer = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); //answer.SplitLines()
                    irbisOpt.OptFormatInfo.fieldId = Convert.ToInt32(OptBuffer[0]);
                    irbisOpt.OptFormatInfo.docTypeMaxLen = Convert.ToInt32(OptBuffer[1]);
                    formatItemsCount = OptBuffer.Length - 3;
                    irbisOpt.OptFormatInfo.formatItems = new FormatItems[formatItemsCount];
                    for (int i = 0; i < formatItemsCount; i++)
                    {
                        txtLine = OptBuffer[i + 2];
                        index = txtLine.IndexOf(' ');
                        if (index == -1)
                            break;
                        irbisOpt.OptFormatInfo.formatItems[i].docType = txtLine.Substring(0, index);
                        while (txtLine[index] == ' ') index++;
                        irbisOpt.OptFormatInfo.formatItems[i].pftFilename = txtLine.Substring(index);
                    }
                    return irbisOpt;

                }
                catch (IrbisException)
                {
                    return null;
                }
            }

            private IrbisSearchEngine _searchEngine;

            public IrbisSearchEngine SearchEngine
            {
                get
                {
                    if (_searchEngine == null)
                        _searchEngine = new IrbisSearchEngine(client);

                    return _searchEngine;
                }
            }

            // <summary>
            /// Имя файла оптимизации с расширением PFT
            /// </summary>
            /// 

            private string _pftOptFile;

            public string PftOptFile
            {
                get
                {
                    if (_pftOptFile == null)
                    {
                        IniFile iniFile = IniFile.ParseText<IniFile>(client._configuration);
                        _pftOptFile = iniFile.Get<String>("MAIN", "PftOpt", "");
                    }
                    return _pftOptFile;
                }
                private set
                {
                    _pftOptFile = value;
                }
            }

            public IrbisDatabase(string Name, string Desc, ManagedClient64 client)
            {
                this.client = client;
                dbName = Name;
                dbDesc = Desc;
                mfn = new int[0];
                _records = new Dictionary<int, IrbisRecord>();
                _length = 0;
            }
        }
        #endregion
    }
}