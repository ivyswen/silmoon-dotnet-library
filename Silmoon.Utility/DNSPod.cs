using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Windows.Forms;
using System.Collections;
using System.Web;
using System.IO;

namespace Silmoon.Utility
{
    [Serializable()]
    public class DNSPod
    {
        protected internal string _baseAPIUri = "https://www.dnspod.com/";
        string _username;
        string _password;
        string _token;
        bool _isLogin = false;
        string _user_agent = "Unknown_SilmoonAssembly/0.0.0.0";
        public string LastServerResult = "";
        int _userID = 0;
        public bool _blackCase = false;
        public string ValidateFrom = "";
        public ArrayList APIHeaders = new ArrayList();

        public bool BlackCase
        {
            get { return _blackCase; }
            set { _blackCase = value; }
        }
        /// <summary>
        /// ��ȡ״̬�Ƿ����Ѿ���¼
        /// </summary>
        public bool IsLogin
        {
            get { return _isLogin; }
            private set { _isLogin = value; }
        }
        /// <summary>
        /// ��ȡ�����õ�ǰ�û���
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        /// <summary>
        /// ��ȡ�������û�����
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        /// <summary>
        /// ���ð������ʹ�õ�Token
        /// </summary>
        public string Token
        {
            get { return _token; }
            private set { _token = value; }
        }
        /// <summary>
        /// ��ȡ������DNSPod API·��
        /// </summary>
        public string BaseAPIUri
        {
            get { return _baseAPIUri; }
            set { _baseAPIUri = value; }
        }
        /// <summary>
        /// ��ȡUA
        /// </summary>
        public string User_Agent
        {
            get { return _user_agent; }
        }
        /// <summary>
        /// ��ȡ�û�ID
        /// </summary>
        public int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        /// <summary>
        /// ���͹�����
        /// </summary>
        public DNSPod()
        {

        }
        /// <summary>
        /// ���͹�������ָ��ʹ�õ�UA
        /// </summary>
        /// <param name="user_agent"></param>
        public DNSPod(string user_agent)
        {
            _user_agent = user_agent;
        }

        /// <summary>
        /// �����������POST����
        /// </summary>
        /// <param name="apiField">API�ֶ�</param>
        /// <param name="data">POST����</param>
        /// <returns>���������ص�����</returns>
        public string GetXml(string apiField, string data)
        {
            WebClient _wclit = new WebClient();
            try
            {
                _wclit.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                _wclit.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                foreach (string s in APIHeaders)
                    _wclit.Headers.Add(s);
                _wclit.Headers.Add("User-Agent", "(SM)" + _user_agent);

                Uri uri = new Uri(_baseAPIUri + "API/" + apiField);
                byte[] bytes = _wclit.UploadData(uri, Encoding.UTF8.GetBytes(data));
                LastServerResult = Encoding.UTF8.GetString(bytes);
                _wclit.Dispose();
                return Encoding.UTF8.GetString(bytes);
            }
            catch (WebException ex)
            {
                try
                {
                    File.WriteAllText(@"C:\DNSPod_Network_Error.xml.txt", DateTime.Now + "\r\n\r\n" + ex + "\r\n\r\n");
                    if (ex.Response != null)
                        File.AppendAllText(@"C:\DNSPod_Network_Error.xml.txt", new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                }
                catch { }
                _wclit.Dispose();
                return "!" + ex.ToString();
            }
        }

        /// <summary>
        /// ָ��ʹ�õ�UA
        /// </summary>
        /// <param name="user_agent"></param>
        public void SetUserAgent(string user_agent)
        {
            _user_agent = user_agent;
        }
        /// <summary>
        /// ָ��DNSPod API·��
        /// </summary>
        /// <param name="apiUrl"></param>
        public void SetAPIUrl(string apiUrl)
        {
            _baseAPIUri = apiUrl;
        }
        /// <summary>
        /// ��¼
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <returns></returns>
        public UserResult Login(string username, string password)
        {
            BlackCase = false;

            _username = username;
            _password = password;
            return GetUserInfo();
        }
        /// <summary>
        /// ���õ�¼״̬
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <param name="login">�Ƿ�����Ϊ�Ѿ���¼</param>
        public void SetLoginState(string username, string password, bool login)
        {
            BlackCase = false;
            _username = username;
            _password = password;
            _isLogin = login;
        }
        /// <summary>
        /// ���������¼
        /// </summary>
        /// <param name="token">TOKEN</param>
        /// <param name="validate">�Ƿ���֤TOKEN</param>
        public UserResult BlackCaseLogin(int userID, string token, bool validate = false)
        {
            BlackCase = true;
            UserID = userID;
            Token = token;
            if (validate)
            {
                return GetUserInfo();
            }
            return null;
        }
        /// <summary>
        /// ���õ�¼״̬
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <param name="login">�Ƿ�����Ϊ�Ѿ���¼</param>
        public void SetBlackLoginState(int userID, string token, bool login)
        {
            BlackCase = true;
            UserID = userID;
            Token = token;
            _isLogin = login;
        }

        /// <summary>
        /// ��ȡ�û�ָ�����͵�����
        /// </summary>
        /// <param name="type">������������</param>
        /// <returns></returns>
        public DomainsResult GetDomains(DomainListType type = DomainListType.all)
        {
            DomainsResult result = new DomainsResult();
            XmlDocument xmlDoc = new XmlDocument();

            string resultXml = GetXml("Domain.List", AuthConnection() + "&type=" + type.ToString());

            if (LoadXml(ref resultXml, result, ref xmlDoc))
            {
                ProcessBaseXml(result, ref xmlDoc, new int[] { 1, 9 });
                ArrayList binfoArr = new ArrayList();
                XmlNode domainsNode = xmlDoc["dnspod"]["domains"];
                if (domainsNode != null)
                {
                    foreach (XmlNode node in domainsNode)
                    {
                        Domain newInfo = new Domain();
                        newInfo.DomainName = node["name"].InnerText;
                        newInfo.State = SmString.StringToBool(node["status"].InnerText);
                        newInfo.Records = int.Parse(node["records"].InnerText);
                        newInfo.ID = int.Parse(node["id"].InnerText);
                        newInfo.Grade = StringToGrade(node["grade"].InnerText);
                        if (node["shared_from"] != null)
                            newInfo.ShardForm = node["shared_from"].InnerText;
                        newInfo.Validate = DNSPodValidateInfo.FromDNSPod;
                        binfoArr.Add(newInfo);
                    }
                }
                result.Domains = (Domain[])binfoArr.ToArray(typeof(Domain));
            }
            return result;
        }
        /// <summary>
        /// ��ȡ�û�������¼
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <returns>������¼����</returns>
        public RecordsResult GetRecords(int domainID)
        {
            XmlDocument xmlDoc = new XmlDocument();
            RecordsResult result = new RecordsResult();
            string resultXml = GetXml("Record.List", AuthConnection() + "&domain_id=" + domainID);

            if (LoadXml(ref resultXml, result, ref xmlDoc))
            {
                ProcessBaseXml(result, ref xmlDoc, new int[] { 1, 10 });
                ArrayList array = new ArrayList();
                XmlNode recordsNode = xmlDoc["dnspod"]["records"];
                if (recordsNode != null)
                {
                    foreach (XmlNode node in recordsNode)
                    {
                        Record newRecord = new Record();
                        newRecord.Enable = SmString.StringToBool(node["enabled"].InnerText);
                        newRecord.ID = int.Parse(node["id"].InnerText);
                        newRecord.Isp = node["line"].InnerText;
                        newRecord.MXLevel = int.Parse(node["mx"].InnerText);
                        newRecord.Subname = node["name"].InnerText;
                        newRecord.TTL = int.Parse(node["ttl"].InnerText);
                        newRecord.Type = DNSPod.StringToRecordType(node["type"].InnerText);
                        newRecord.Value = node["value"].InnerText;
                        newRecord.Validate = DNSPodValidateInfo.FromDNSPod;
                        array.Add(newRecord);
                    }
                }
                result.Records = (Record[])array.ToArray(typeof(Record));
            }
            return result;
        }
        /// <summary>
        /// ������ID�ͼ�¼ID��ȡ��¼��Ϣ
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="recordID">��¼ID</param>
        /// <returns></returns>
        public RecordResult GetRecord(int domainID, int recordID)
        {
            XmlDocument xmlDoc = new XmlDocument();
            RecordResult result = new RecordResult();
            string resultXml = GetXml("Record.Info", AuthConnection() + "&domain_id=" + domainID + "&record_id=" + recordID);

            if (LoadXml(ref resultXml, result, ref xmlDoc))
            {
                ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });

                XmlNode node = xmlDoc["dnspod"]["record"];
                result.Record = new Record();
                result.Record.Enable = SmString.StringToBool(node["enabled"].InnerText);
                result.Record.ID = int.Parse(node["id"].InnerText);
                result.Record.Subname = node["sub_domain"].InnerText;
                result.Record.Isp = node["record_line"].InnerText;
                result.Record.Type = DNSPod.StringToRecordType(node["record_type"].InnerText);
                result.Record.Validate = DNSPodValidateInfo.FromDNSPod;
                result.Record.Value = node["value"].InnerText;
                result.Record.MXLevel = int.Parse(node["mx"].InnerText);
                result.Record.TTL = int.Parse(node["ttl"].InnerText);
            }
            return result;
        }
        /// <summary>
        /// �Ӽ�¼�����л�ȡһ����¼����Ϣ��
        /// </summary>
        /// <param name="records">��¼����</param>
        /// <param name="recordID">��¼ID</param>
        /// <returns></returns>
        public Record GetRecord(Record[] records, int recordID)
        {
            foreach (Record record in records)
            {
                if (record.ID == recordID)
                    return record;
            }
            return null;
        }
        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="enable">�Ƿ�����</param>
        /// <returns></returns>
        public DNSPodResult SetDomainState(int domainID, bool enable)
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string enableArgs = "";
            if (enable) enableArgs = "enable"; else enableArgs = "disable";
            string resultXml = GetXml("Domain.Status", AuthConnection() + "&domain_id=" + domainID + "&status=" + enableArgs);

            LoadXml(ref resultXml, result, ref xmlDoc);
            ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
            result.Message = xmlDoc.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="domain">Ҫ��ӵ�����</param>
        /// <returns></returns>
        public DNSPodResult CreateDomain(string domain)
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string resultXml = GetXml("Domain.Create", AuthConnection() + "&domain=" + domain);
            LoadXml(ref resultXml, result, ref xmlDoc);
            ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
            if (!result.Error)
                result.Data = int.Parse(xmlDoc["dnspod"]["domain"]["id"].InnerText);
            return result;
        }
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="domainID">Ҫɾ��������ID</param>
        /// <returns></returns>
        public DNSPodResult RemoveDomain(int domainID)
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string resultXml = GetXml("Domain.Remove", AuthConnection() + "&domain_id=" + domainID);
            LoadXml(ref resultXml, result, ref xmlDoc);
            ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
            return result;
        }
        /// <summary>
        /// ��Ӽ�¼
        /// </summary>
        /// <param name="record">������¼��Ϣ</param>
        /// <param name="domainID">����ID</param>
        /// <returns>״̬��</returns>
        public DNSPodResult CreateRecord(Record record, int domainID)
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string urlArgs = AuthConnection();
            urlArgs += "&domain_id=" + domainID;
            urlArgs += "&sub_domain=" + record.Subname;
            urlArgs += "&record_type=" + record.Type.ToString();
            urlArgs += "&record_line=" + record.Isp.ToString().ToLower();
            urlArgs += "&value=" + HttpUtility.UrlEncode(record.Value);
            urlArgs += "&mx=" + record.MXLevel;
            urlArgs += "&ttl=" + record.TTL;
            string resultXml = GetXml("Record.Create", urlArgs);
            LoadXml(ref resultXml, result, ref xmlDoc);
            ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
            if (!result.Error)
                result.Data = xmlDoc["dnspod"]["record"]["id"].InnerText;
            return result;
        }
        /// <summary>
        /// �༭������¼
        /// </summary>
        /// <param name="record">������¼��Ϣ</param>
        /// <param name="domainID">����ID</param>
        /// <returns>״̬��</returns>
        public DNSPodResult ModifyRecord(Record record, int domainID)
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string urlArgs = AuthConnection();
            urlArgs += "&domain_id=" + domainID;
            urlArgs += "&record_id=" + record.ID;
            urlArgs += "&sub_domain=" + record.Subname;
            urlArgs += "&record_type=" + record.Type.ToString();
            urlArgs += "&record_line=" + record.Isp.ToString().ToLower();
            urlArgs += "&value=" + HttpUtility.UrlEncode(record.Value);
            urlArgs += "&mx=" + record.MXLevel;
            urlArgs += "&ttl=" + record.TTL;
            urlArgs += "&from=" + ValidateFrom;
            string resultXml = GetXml("Record.Modify", urlArgs);
            LoadXml(ref resultXml, result, ref xmlDoc);
            ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
            return result;
        }
        /// <summary>
        /// ɾ��������¼
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="recordID">��¼ID</param>
        /// <returns>�����</returns>
        public DNSPodResult RemoveRecord(int domainID, int recordID)
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string resultXml = GetXml("Record.Remove", AuthConnection() + "&record_id=" + recordID + "&domain_id=" + domainID);
            LoadXml(ref resultXml, result, ref xmlDoc);
            ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
            return result;
        }
        /// <summary>
        /// ����������¼״̬
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="recordID">��¼ID</param>
        /// <param name="enable">�Ƿ�����</param>
        /// <returns>�����</returns>
        public DNSPodResult SetRecordState(int domainID, int recordID, bool enable)
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string enableArgs = "";
            if (enable) enableArgs = "enable"; else enableArgs = "disable";
            string resultXml = GetXml("Record.Status", AuthConnection() + "&record_id=" + recordID + "&domain_id=" + domainID + "&status=" + enableArgs);
            LoadXml(ref resultXml, result, ref xmlDoc);
            ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
            return result;
        }

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <returns>������Ϣ</returns>
        public DomainResult GetDomainInfo(int domainID)
        {
            DomainResult result = new DomainResult();
            DomainsResult domains = GetDomains();
            result.IntFlag = domains.IntFlag;
            result.Message = domains.Message;
            result.XML = domains.XML;
            result.Error = domains.Error;

            if (domains.Error) return result;
            foreach (Domain domain in domains.Domains)
            {
                if (domain.ID == domainID)
                    result.Domain = domain;
            }
            return result;
        }
        /// <summary>
        /// ��ȡ������Ϣ��������Ϣ������
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="domains">������Ϣ����</param>
        /// <returns>������Ϣ</returns>
        public Domain GetDomainInfo(int domainID, Domain[] domains)
        {
            foreach (Domain domain in domains)
            {
                if (domain.ID == domainID)
                    return domain;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="domain">����</param>
        /// <returns>������Ϣ</returns>
        public DomainResult GetDomainInfo(string domain)
        {
            DomainResult result = new DomainResult();
            DomainsResult domains = GetDomains();
            result.IntFlag = domains.IntFlag;
            result.Message = domains.Message;
            result.XML = domains.XML;
            result.Error = domains.Error;

            if (domains.Error) return result;
            foreach (Domain domainInfo in domains.Domains)
            {
                if (domainInfo.DomainName.ToLower() == domain.ToLower())
                    result.Domain = domainInfo;
            }
            return result;
        }
        /// <summary>
        /// ��ȡ������Ϣ��������Ϣ������
        /// </summary>
        /// <param name="domain">����</param>
        /// <param name="domains">������Ϣ����</param>
        /// <returns>������Ϣ</returns>
        public Domain GetDomainInfo(string domain, Domain[] domains)
        {
            foreach (Domain domainInfo in domains)
            {
                if (domainInfo.DomainName.ToLower() == domain.ToLower())
                    return domainInfo;
            }
            return null;
        }

        /// <summary>
        /// ��ȡ�û���Ϣ
        /// </summary>
        /// <returns></returns>
        public UserResult GetUserInfo()
        {
            XmlDocument xmlDoc = new XmlDocument();
            UserResult result = new UserResult();
            string resultXml = GetXml("User.Info", AuthConnection());
            if (LoadXml(ref resultXml, result, ref xmlDoc))
            {
                result.User = new DNSPodUser();

                ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
                if (result.IntFlag == 1)
                {
                    result.User.LoginOK = true;
                    IsLogin = true;
                    result.User.UserID = int.Parse(xmlDoc["dnspod"]["user"]["id"].InnerText);
                    result.User.Username = xmlDoc["dnspod"]["user"]["email"].InnerText;
                    Username = result.User.Username;
                    UserID = result.User.UserID;
                }
                else
                {
                    result.User.LoginOK = false;
                    IsLogin = false;
                }
            }

            return result;
        }
        /// <summary>
        /// ��ȡ�û�TOKEN
        /// </summary>
        /// <param name="userID">����ǹ���Աģʽ�����Ի�ȡ�����û���TOKEN��ѡ�����</param>
        /// <returns></returns>
        public DNSPodResult GetUserToken(int userID = 0)
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string resultXml = GetXml("User.Token", AuthConnection());
            if (LoadXml(ref resultXml, result, ref xmlDoc))
            {
                ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
                result.Data = xmlDoc["dnspod"]["user"]["login_token"].InnerText;
                _token = result.Data.ToString();
            }
            return result;
        }
        /// <summary>
        /// ��ȡDNSPodһ��������
        /// </summary>
        /// <returns>һ��������</returns>
        public DNSPodResult GetOncePassword()
        {
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();
            string resultXml = GetXml("Login.Key", AuthConnection());
            if (LoadXml(ref resultXml, result, ref xmlDoc))
            {
                ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
                result.Data = xmlDoc.GetElementsByTagName("key")[0].InnerText;
            }
            return result;
        }
        /// <summary>
        /// ��ȡ�����Ŀ�����·
        /// </summary>
        /// <param name="domainInfo">������Ϣ</param>
        /// <returns></returns>
        public DNSPodResult GetDomainNetworkType(Domain domainInfo)
        {
            string resultXml = GetXml("Record.Line", AuthConnection() + "&domain_grade=" + domainInfo.Grade);
            XmlDocument xmlDoc = new XmlDocument();
            DNSPodResult result = new DNSPodResult();

            if (LoadXml(ref resultXml, result, ref xmlDoc))
            {
                ArrayList array = new ArrayList();
                ProcessBaseXml(result, ref xmlDoc, new int[] { 1 });
                XmlNode xmlNode = xmlDoc["dnspod"]["lines"];
                foreach (XmlNode item in xmlNode)
                {
                    array.Add(item.InnerText);
                }
                result.Data = (string[])array.ToArray(typeof(string));
            }
            return result;
        }
        /// <summary>
        /// Load Xml from string
        /// </summary>
        /// <param name="xmlString">string</param>
        /// <param name="xmlDoc">xml object</param>
        public bool LoadXml(ref string xmlString, DNSPodResult result, ref XmlDocument xmlDoc)
        {
            bool success = false;
            try
            {
                xmlDoc.LoadXml(xmlString);
                success = true;
                result.XML = xmlString;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.IntFlag = -9999;
                result.Message = ex.ToString();
                result.ObjectReferer = ex;
                success = false;
            }
            return success;
        }
        /// <summary>
        /// ����DNSPod���صĻ�������
        /// </summary>
        /// <param name="result">״̬����</param>
        /// <param name="xmlDoc">XML����</param>
        /// <param name="noWaringCode">�޴��״̬��������</param>
        /// <returns></returns>
        public bool ProcessBaseXml(DNSPodResult result, ref XmlDocument xmlDoc, int[] noWaringCode)
        {
            result.Error = true;
            try
            {
                result.IntFlag = int.Parse(xmlDoc["dnspod"]["status"]["code"].InnerText);
                result.Message = xmlDoc["dnspod"]["status"]["message"].InnerText;
                if (result.IntFlag == 1) result.Error = false;
                foreach (int item in noWaringCode)
                {
                    if (result.IntFlag == item)
                    {
                        result.Error = false;
                        break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                result.Error = true;
                return false;
            }
        }

        public static RecordType StringToRecordType(string type)
        {
            switch (type.ToLower())
            {
                case "a":
                    return RecordType.A;
                case "cname":
                    return RecordType.CNAME;
                case "mx":
                    return RecordType.MX;
                case "url":
                    return RecordType.URL;
                case "ns":
                    return RecordType.NS;
                case "txt":
                    return RecordType.TXT;
                case "aaaa":
                    return RecordType.AAAA;
                case "cn":
                    return RecordType.CNAME;
                case "v6":
                    return RecordType.AAAA;
                default:
                    return RecordType.A;
            }
        }
        public static DomainGrade StringToGrade(string grade)
        {
            switch (grade.ToLower())
            {
                case "d_free":
                    return DomainGrade.D_Free;
                case "d_express":
                    return DomainGrade.D_Express;
                case "d_plus":
                    return DomainGrade.D_Plus;
                case "d_extra":
                    return DomainGrade.D_Extra;
                case "d_expert":
                    return DomainGrade.D_Expert;
                case "d_ultra":
                    return DomainGrade.D_Ultra;
                default:
                    return DomainGrade.D_Free;
            }
        }

        public string AuthConnection()
        {
            if (BlackCase)
            {
                return "login_id=" + UserID + "&login_token=" + Token + "&format=xml";
            }
            else
            {
                return "login_email=" + HttpUtility.UrlEncode(_username) + "&login_password=" + HttpUtility.UrlEncode(_password) + "&format=xml";
            }
        }

        public enum DomainListType
        {
            all, share, mine
        }
    }
    public class Domain
    {
        public string DomainName;
        public bool State;
        public int Records;
        public DomainGrade Grade;
        public int ID;
        public string ShardForm;
        public DNSPodValidateInfo Validate = DNSPodValidateInfo.New;
    }
    public class DomainsResult : DNSPodResult
    {
        public Domain[] Domains;
    }
    public class DomainResult : DNSPodResult
    {
        public Domain Domain;
    }

    public enum DomainGrade
    {
        D_Free = 0,
        D_Express = 1,
        D_Plus = 2,
        D_Extra = 3,
        D_Expert = 4,
        D_Ultra = 5
    }

    public class Record
    {
        public int ID;
        public string Subname;
        public string Isp = "Ĭ��";
        public RecordType Type;
        public int TTL = 3600;
        public string Value;
        public int MXLevel = 5;
        public bool Enable = true;
        public DNSPodValidateInfo Validate = DNSPodValidateInfo.New;
        public string Note;
        public Record() { }
        public Record(string subname, string isp, RecordType type, string value)
        {
            Subname = subname;
            Isp = isp;
            Type = type;
            Value = value;
        }
    }
    public class RecordsResult : DNSPodResult
    {
        public Record[] Records;
    }
    public class RecordResult : DNSPodResult
    {
        public Record Record;
    }

    public class DNSPodUser
    {
        public string Username;
        public int UserID;
        public string UserToken;
        public bool LoginOK = false;
        public string Message;
    }
    public class UserResult : DNSPodResult
    {
        public DNSPodUser User;
    }

    public enum AgentGrade
    {
        unknown = 0,
        bronze = 1,
        silver = 2,
        gold = 3,
        diamond = 4,
    }

    public enum RecordType
    {
        A = 1,
        CNAME = 2,
        MX = 3,
        URL = 4,
        NS = 5,
        TXT = 6,
        AAAA = 7,
    }
    public enum DNSPodValidateInfo
    {
        FromDNSPod,
        Invalid,
        New,
    }

    public class DNSPodResult : IStateFlag
    {
        bool _error = false;
        int _intFlag = -9999;
        string _message = "message not defined";
        object _objectReferer = null;
        public string XML = "";
        public object Data;

        #region IStateFlag ��Ա

        public bool Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }

        public int IntFlag
        {
            get
            {
                return _intFlag;
            }
            set
            {
                _intFlag = value;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        public object ObjectReferer
        {
            get
            {
                return _objectReferer;
            }
            set
            {
                _objectReferer = value;
            }
        }

        #endregion
    }
}