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
        public string _result = "";
        int _userID = 0;
        public bool BlackCase = false;
        public ArrayList APIHeaders = new ArrayList();

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
        public string GetDNSPodServerXml(string apiField, string data)
        {
            WebClient _wclit = new WebClient();
            try
            {
                _wclit.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                _wclit.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                foreach (string s in APIHeaders)
                    _wclit.Headers.Add(s);
                _wclit.Headers.Add("User-Agent", "(SM)" + _user_agent);
                byte[] bytes = _wclit.UploadData(new Uri(_baseAPIUri + "API/" + apiField), Encoding.UTF8.GetBytes(data));
                _result = Encoding.UTF8.GetString(bytes);
                _wclit.Dispose();
                return Encoding.UTF8.GetString(bytes);
            }
            catch (WebException ex)
            {
                try
                {
                    File.WriteAllText(@"C:\DNSPodClientErrXmlString.xml.txt", DateTime.Now + "\r\n\r\n" + ex + "\r\n\r\n");
                    if (ex.Response != null)
                        File.AppendAllText(@"C:\DNSPodClientErrXmlString.xml.txt", new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
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
        public UserInfo Login(string username, string password)
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
        public UserInfo BlackCaseLogin(int userID, string token, bool validate = false)
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
        /// ��ȡ�û�����������
        /// </summary>
        /// <returns></returns>
        public DomainInfo[] GetDomains()
        {
            return GetDomains(DomainListType.all);
        }
        /// <summary>
        /// ��ȡ�û�ָ�����͵�����
        /// </summary>
        /// <param name="type">������������</param>
        /// <returns></returns>
        public DomainInfo[] GetDomains(DomainListType type)
        {
            XmlDocument _xml = new XmlDocument();

            string resultXml = GetDNSPodServerXml("Domain.List", AuthConnection() + "&type=" + type.ToString());
            if (resultXml == "") return null;

            LoadXml(ref resultXml, ref _xml);
            ArrayList binfoArr = new ArrayList();
            XmlNode domainsNode = _xml["dnspod"]["domains"];

            foreach (XmlNode node in domainsNode)
            {
                DomainInfo newInfo = new DomainInfo();
                newInfo.DomainName = node["name"].InnerText;
                newInfo.State = SmString.StringToBool(node["status"].InnerText);
                newInfo.Records = int.Parse(node["records"].InnerText);
                newInfo.ID = int.Parse(node["id"].InnerText);
                newInfo.Grade = StringToGrade(node["grade"].InnerText);
                if (node["shared_from"] != null)
                    newInfo.ShardForm = node["shared_from"].InnerText;
                newInfo.Validate = DNSPodUnitValidate.FromDNSPod;
                binfoArr.Add(newInfo);
            }
            return (DomainInfo[])binfoArr.ToArray(typeof(DomainInfo));
        }
        /// <summary>
        /// ��ȡ�û�������¼
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <returns>������¼����</returns>
        public RecordInfo[] GetRecords(int domainID)
        {
            XmlDocument _xml = new XmlDocument();
            ArrayList array = new ArrayList();

            string resultXml = GetDNSPodServerXml("Record.List", AuthConnection() + "&domain_id=" + domainID);
            if (resultXml == "") return null;
            LoadXml(ref resultXml, ref _xml);

            if (_xml.GetElementsByTagName("code")[0].InnerText == "1" || _xml.GetElementsByTagName("code")[0].InnerText == "7")
            {
                XmlNode recordsNode = _xml["dnspod"]["records"];
                foreach (XmlNode node in recordsNode)
                {
                    RecordInfo newRecord = new RecordInfo();
                    newRecord.Enable = SmString.StringToBool(node["enabled"].InnerText);
                    newRecord.ID = int.Parse(node["id"].InnerText);
                    newRecord.Isp = DNSPod.StringToISP(node["line"].InnerText);
                    newRecord.MXLevel = int.Parse(node["mx"].InnerText);
                    newRecord.Subname = node["name"].InnerText;
                    newRecord.TTL = int.Parse(node["ttl"].InnerText);
                    newRecord.Type = DNSPod.StringToRecordType(node["type"].InnerText);
                    newRecord.Value = node["value"].InnerText;
                    newRecord.Validate = DNSPodUnitValidate.FromDNSPod;
                    array.Add(newRecord);
                }
            }
            return (RecordInfo[])array.ToArray(typeof(RecordInfo));
        }
        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="enable">�Ƿ�����</param>
        /// <returns></returns>
        public StateFlag SetDomainState(int domainID, bool enable)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string enableArgs = "";
            if (enable) enableArgs = "enable"; else enableArgs = "disable";
            string resultXml = GetDNSPodServerXml("Domain.Status", AuthConnection() + "&domain_id=" + domainID + "&status=" + enableArgs);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="domain">Ҫ��ӵ�����</param>
        /// <returns></returns>
        public StateFlag CreateDomain(string domain)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string resultXml = GetDNSPodServerXml("Domain.Create", AuthConnection() + "&domain=" + domain);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="domainID">Ҫɾ��������ID</param>
        /// <returns></returns>
        public StateFlag RemoveDomain(int domainID)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string resultXml = GetDNSPodServerXml("Domain.Remove", AuthConnection() + "&domain_id=" + domainID);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ��Ӽ�¼
        /// </summary>
        /// <param name="record">������¼��Ϣ</param>
        /// <param name="domainID">����ID</param>
        /// <returns>״̬��</returns>
        public StateFlag CreateRecord(RecordInfo record, int domainID)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string urlArgs = AuthConnection();
            urlArgs += "&domain_id=" + domainID;
            urlArgs += "&sub_domain=" + record.Subname;
            urlArgs += "&record_type=" + record.Type.ToString();
            urlArgs += "&record_line=" + record.Isp.ToString().ToLower();
            urlArgs += "&value=" + HttpUtility.UrlEncode(record.Value);
            urlArgs += "&mx=" + record.MXLevel;
            urlArgs += "&ttl=" + record.TTL;
            string resultXml = GetDNSPodServerXml("Record.Create", urlArgs);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            if (_xml.GetElementsByTagName("id").Count == 0)
                result.ID = 0;
            else
                result.ID = int.Parse(_xml.GetElementsByTagName("id")[0].InnerText);

            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// �༭������¼
        /// </summary>
        /// <param name="record">������¼��Ϣ</param>
        /// <param name="domainID">����ID</param>
        /// <returns>״̬��</returns>
        public StateFlag ModifyRecord(RecordInfo record, int domainID)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string urlArgs = AuthConnection();
            urlArgs += "&domain_id=" + domainID;
            urlArgs += "&record_id=" + record.ID;
            urlArgs += "&sub_domain=" + record.Subname;
            urlArgs += "&record_type=" + record.Type.ToString();
            urlArgs += "&record_line=" + record.Isp.ToString().ToLower();
            urlArgs += "&value=" + HttpUtility.UrlEncode(record.Value);
            urlArgs += "&mx=" + record.MXLevel;
            urlArgs += "&ttl=" + record.TTL;
            string resultXml = GetDNSPodServerXml("Record.Modify", urlArgs);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ɾ��������¼
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="recordID">��¼ID</param>
        /// <returns>�����</returns>
        public StateFlag RemoveRecord(int domainID, int recordID)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string resultXml = GetDNSPodServerXml("Record.Remove", AuthConnection() + "&record_id=" + recordID + "&domain_id=" + domainID);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }

            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ����������¼״̬
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="recordID">��¼ID</param>
        /// <param name="enable">�Ƿ�����</param>
        /// <returns>�����</returns>
        public StateFlag SetRecordState(int domainID, int recordID, bool enable)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string enableArgs = "";
            if (enable) enableArgs = "enable"; else enableArgs = "disable";
            string resultXml = GetDNSPodServerXml("Record.Status", AuthConnection() + "&record_id=" + recordID + "&domain_id=" + domainID + "&status=" + enableArgs);
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <returns>������Ϣ</returns>
        public DomainInfo GetDomainInfo(int domainID)
        {
            DomainInfo[] domains = GetDomains();
            foreach (DomainInfo domain in domains)
            {
                if (domain.ID == domainID)
                    return domain;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ������Ϣ��������Ϣ������
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="domains">������Ϣ����</param>
        /// <returns>������Ϣ</returns>
        public DomainInfo GetDomainInfo(int domainID, DomainInfo[] domains)
        {
            foreach (DomainInfo domain in domains)
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
        public DomainInfo GetDomainInfo(string domain)
        {
            DomainInfo[] domains = GetDomains();
            foreach (DomainInfo domainInfo in domains)
            {
                if (domainInfo.DomainName.ToLower() == domain.ToLower())
                    return domainInfo;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ������Ϣ��������Ϣ������
        /// </summary>
        /// <param name="domain">����</param>
        /// <param name="domains">������Ϣ����</param>
        /// <returns>������Ϣ</returns>
        public DomainInfo GetDomainInfo(string domain, DomainInfo[] domains)
        {
            foreach (DomainInfo domainInfo in domains)
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
        public UserInfo GetUserInfo()
        {
            XmlDocument _xml = new XmlDocument();

            UserInfo userInfo = new UserInfo();
            string resultXml = GetDNSPodServerXml("User.Info", AuthConnection());
            if (resultXml == "")
                userInfo.StateCode = -99;
            else
            {
                LoadXml(ref resultXml, ref _xml);
                userInfo.StateCode = int.Parse(_xml["dnspod"]["status"]["code"].InnerText);
                userInfo.Message = _xml["dnspod"]["status"]["message"].InnerText;
                if (userInfo.StateCode == 1)
                {
                    userInfo.LoginOK = true;
                    IsLogin = true;
                    userInfo.UserID = int.Parse(_xml["dnspod"]["user"]["id"].InnerText);
                    userInfo.Username = _xml["dnspod"]["user"]["email"].InnerText;
                    Username = userInfo.Username;
                }
                else
                {
                    userInfo.LoginOK = false;
                    IsLogin = false;
                }
            }

            return userInfo;
        }
        /// <summary>
        /// ��ȡDNSPodһ��������
        /// </summary>
        /// <returns>һ��������</returns>
        public string GetOncePassword()
        {
            XmlDocument _xml = new XmlDocument();

            string resultXml = GetDNSPodServerXml("Login.Key", AuthConnection());
            LoadXml(ref resultXml, ref _xml);
            if (_xml.GetElementsByTagName("code")[0].InnerText == "1")
            {
                return _xml.GetElementsByTagName("key")[0].InnerText;
            } return "";
        }
        /// <summary>
        /// Load Xml from string
        /// </summary>
        /// <param name="xmlString">string</param>
        /// <param name="xmlDoc">xml object</param>
        public void LoadXml(ref string xmlString, ref XmlDocument xmlDoc)
        {
            try
            {
                xmlDoc.LoadXml(xmlString);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public static ISP StringToISP(string isp)
        {

            switch (isp.ToLower())
            {
                case "tel":
                    return ISP.TEL;
                case "cnc":
                    return ISP.CNC;
                case "cuc":
                    return ISP.CNC;
                case "edu":
                    return ISP.EDU;
                case "cmc":
                    return ISP.CMC;
                case "foreign":
                    return ISP.FOREIGN;
                case "for":
                    return ISP.FOREIGN;
                case "����":
                    return ISP.TEL;
                case "��ͨ":
                    return ISP.CNC;
                case "��ͨ":
                    return ISP.CNC;
                case "����":
                    return ISP.EDU;
                case "�ƶ�":
                    return ISP.CMC;
                case "����":
                    return ISP.FOREIGN;
                case "anhui_tel":
                    return ISP.anhui_tel;
                case "anhui_cnc":
                    return ISP.anhui_cnc;
                case "aomen":
                    return ISP.aomen;
                case "beijing_tel":
                    return ISP.beijing_tel;
                case "beijing_cnc":
                    return ISP.beijing_cnc;
                case "chongqing_tel":
                    return ISP.chongqing_tel;
                case "chongqing_cnc":
                    return ISP.chongqing_cnc;
                case "fujian_tel":
                    return ISP.fujian_tel;
                case "fujian_cnc":
                    return ISP.fujian_cnc;
                case "gansu_tel":
                    return ISP.gansu_tel;
                case "gansu_cnc":
                    return ISP.gansu_cnc;
                case "guangdong_tel":
                    return ISP.guangdong_tel;
                case "guangdong_cnc":
                    return ISP.guangdong_cnc;
                case "guangxi_tel":
                    return ISP.guangxi_tel;
                case "guangxi_cnc":
                    return ISP.guangxi_cnc;
                case "guizhou_tel":
                    return ISP.guizhou_tel;
                case "guizhou_cnc":
                    return ISP.guizhou_cnc;
                case "hainan_tel":
                    return ISP.hainan_tel;
                case "hainan_cnc":
                    return ISP.hainan_cnc;
                case "hebei_tel":
                    return ISP.hebei_tel;
                case "hebei_cnc":
                    return ISP.hebei_cnc;
                case "henan_tel":
                    return ISP.henan_tel;
                case "henan_cnc":
                    return ISP.henan_cnc;
                case "heilongjiang_tel":
                    return ISP.heilongjiang_tel;
                case "heilongjiang_cnc":
                    return ISP.heilongjiang_cnc;
                case "hubei_tel":
                    return ISP.hubei_tel;
                case "hubei_cnc":
                    return ISP.hubei_cnc;
                case "hunan_tel":
                    return ISP.hunan_tel;
                case "hunan_cnc":
                    return ISP.hunan_cnc;
                case "jilin_tel":
                    return ISP.jilin_tel;
                case "jilin_cnc":
                    return ISP.jilin_cnc;
                case "jiangsu_tel":
                    return ISP.jiangsu_tel;
                case "jiangsu_cnc":
                    return ISP.jiangsu_cnc;
                case "jiangxi_tel":
                    return ISP.jiangxi_tel;
                case "jiangxi_cnc":
                    return ISP.jiangxi_cnc;
                case "liaoning_tel":
                    return ISP.liaoning_tel;
                case "liaoning_cnc":
                    return ISP.liaoning_cnc;
                case "neimeng_tel":
                    return ISP.neimeng_tel;
                case "neimeng_cnc":
                    return ISP.neimeng_cnc;
                case "ningxia_tel":
                    return ISP.ningxia_tel;
                case "ningxia_cnc":
                    return ISP.ningxia_cnc;
                case "qinghai_tel":
                    return ISP.qinghai_tel;
                case "qinghai_cnc":
                    return ISP.qinghai_cnc;
                case "shandong_tel":
                    return ISP.shandong_tel;
                case "shandong_cnc":
                    return ISP.shandong_cnc;
                case "shanxi_tel":
                    return ISP.shanxi_tel;
                case "shanxi_cnc":
                    return ISP.shanxi_cnc;
                case "shaanxi_tel":
                    return ISP.shaanxi_tel;
                case "shaanxi_cnc":
                    return ISP.shaanxi_cnc;
                case "shanghai_tel":
                    return ISP.shanghai_tel;
                case "shanghai_cnc":
                    return ISP.shanghai_cnc;
                case "sichuan_tel":
                    return ISP.sichuan_tel;
                case "sichuan_cnc":
                    return ISP.sichuan_cnc;
                case "taiwan":
                    return ISP.taiwan;
                case "tianjin_tel":
                    return ISP.tianjin_tel;
                case "tianjin_cnc":
                    return ISP.tianjin_cnc;
                case "xizang_tel":
                    return ISP.xizang_tel;
                case "xizang_cnc":
                    return ISP.xizang_cnc;
                case "xianggang":
                    return ISP.xianggang;
                case "xinjiang_tel":
                    return ISP.xinjiang_tel;
                case "xinjiang_cnc":
                    return ISP.xinjiang_cnc;
                case "yunnan_tel":
                    return ISP.yunnan_tel;
                case "yunnan_cnc":
                    return ISP.yunnan_cnc;
                case "zhejiang_tel":
                    return ISP.zhejiang_tel;
                case "zhejiang_cnc":
                    return ISP.zhejiang_cnc;
                default:
                    return ISP.DEFAULT;
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
            switch (grade)
            {
                case "Free":
                    return DomainGrade.Free;
                case "Express":
                    return DomainGrade.Express;
                case "Extra":
                    return DomainGrade.Extra;
                case "Ultra":
                    return DomainGrade.Ultra;
                default:
                    return DomainGrade.Free;
            }
        }

        public static string ISPToText(ISP isp)
        {
            switch (isp)
            {
                case ISP.DEFAULT:
                    return "";
                case ISP.TEL:
                    return "����";
                case ISP.CNC:
                    return "��ͨ";
                case ISP.EDU:
                    return "����";
                case ISP.CMC:
                    return "�ƶ�";
                case ISP.FOREIGN:
                    return "����";
                case ISP.anhui_tel:
                    return "���յ���";
                case ISP.anhui_cnc:
                    return "������ͨ";
                case ISP.aomen:
                    return "����";
                case ISP.beijing_tel:
                    return "��������";
                case ISP.beijing_cnc:
                    return "������ͨ";
                case ISP.chongqing_tel:
                    return "�������";
                case ISP.chongqing_cnc:
                    return "������ͨ";
                case ISP.fujian_tel:
                    return "��������";
                case ISP.fujian_cnc:
                    return "������ͨ";
                case ISP.gansu_tel:
                    return "�������";
                case ISP.gansu_cnc:
                    return "������ͨ";
                case ISP.guangdong_tel:
                    return "�㶫����";
                case ISP.guangdong_cnc:
                    return "�㶫��ͨ";
                case ISP.guangxi_tel:
                    return "��������";
                case ISP.guangxi_cnc:
                    return "������ͨ";
                case ISP.guizhou_tel:
                    return "���ݵ���";
                case ISP.guizhou_cnc:
                    return "������ͨ";
                case ISP.hainan_tel:
                    return "���ϵ���";
                case ISP.hainan_cnc:
                    return "������ͨ";
                case ISP.hebei_tel:
                    return "�ӱ�����";
                case ISP.hebei_cnc:
                    return "�ӱ���ͨ";
                case ISP.henan_tel:
                    return "���ϵ���";
                case ISP.henan_cnc:
                    return "������ͨ";
                case ISP.heilongjiang_tel:
                    return "����������";
                case ISP.heilongjiang_cnc:
                    return "��������ͨ";
                case ISP.hubei_tel:
                    return "��������";
                case ISP.hubei_cnc:
                    return "������ͨ";
                case ISP.hunan_tel:
                    return "���ϵ���";
                case ISP.hunan_cnc:
                    return "������ͨ";
                case ISP.jilin_tel:
                    return "���ֵ���";
                case ISP.jilin_cnc:
                    return "������ͨ";
                case ISP.jiangsu_tel:
                    return "���յ���";
                case ISP.jiangsu_cnc:
                    return "������ͨ";
                case ISP.jiangxi_tel:
                    return "��������";
                case ISP.jiangxi_cnc:
                    return "������ͨ";
                case ISP.liaoning_tel:
                    return "��������";
                case ISP.liaoning_cnc:
                    return "������ͨ";
                case ISP.neimeng_tel:
                    return "���ɵ���";
                case ISP.neimeng_cnc:
                    return "������ͨ";
                case ISP.ningxia_tel:
                    return "���ĵ���";
                case ISP.ningxia_cnc:
                    return "������ͨ";
                case ISP.qinghai_tel:
                    return "�ຣ����";
                case ISP.qinghai_cnc:
                    return "�ຣ��ͨ";
                case ISP.shandong_tel:
                    return "ɽ������";
                case ISP.shandong_cnc:
                    return "ɽ����ͨ";
                case ISP.shanxi_tel:
                    return "ɽ������";
                case ISP.shanxi_cnc:
                    return "ɽ����ͨ";
                case ISP.shaanxi_tel:
                    return "��������";
                case ISP.shaanxi_cnc:
                    return "������ͨ";
                case ISP.shanghai_tel:
                    return "�Ϻ�����";
                case ISP.shanghai_cnc:
                    return "�Ϻ���ͨ";
                case ISP.sichuan_tel:
                    return "�Ĵ�����";
                case ISP.sichuan_cnc:
                    return "�Ĵ���ͨ";
                case ISP.taiwan:
                    return "̨��";
                case ISP.tianjin_tel:
                    return "������";
                case ISP.tianjin_cnc:
                    return "�����ͨ";
                case ISP.xizang_tel:
                    return "���ص���";
                case ISP.xizang_cnc:
                    return "������ͨ";
                case ISP.xianggang:
                    return "���";
                case ISP.xinjiang_tel:
                    return "�½�����";
                case ISP.xinjiang_cnc:
                    return "�½���ͨ";
                case ISP.yunnan_tel:
                    return "���ϵ���";
                case ISP.yunnan_cnc:
                    return "������ͨ";
                case ISP.zhejiang_tel:
                    return "�㽭����";
                case ISP.zhejiang_cnc:
                    return "�㽭��ͨ";
                default:
                    return isp.ToString();
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
    public class DomainInfo
    {
        public string DomainName;
        public bool State;
        public int Records;
        public DomainGrade Grade;
        public int ID;
        public string ShardForm;
        public DNSPodUnitValidate Validate = DNSPodUnitValidate.New;
    }
    public enum DomainGrade
    {
        Free = 0,
        Express = 1,
        Extra = 2,
        Ultra = 3
    }
    public class RecordInfo
    {
        public int ID;
        public string Subname;
        public ISP Isp = ISP.DEFAULT;
        public RecordType Type;
        public int TTL = 3600;
        public string Value;
        public int MXLevel = 5;
        public bool Enable = true;
        public DNSPodUnitValidate Validate = DNSPodUnitValidate.New;
        public string Note;
        public RecordInfo() { }
        public RecordInfo(string subname, ISP isp, RecordType type, string value)
        {
            Subname = subname;
            Isp = isp;
            Type = type;
            Value = value;
        }
    }
    public class UserInfo
    {
        public string Username;
        public int UserID;
        public int StateCode = -99;
        public bool LoginOK = false;
        public string Message;
    }
    public enum AgentGrade
    {
        unknown = 0,
        bronze = 1,
        silver = 2,
        gold = 3,
        diamond = 4,
    }
    public enum ISP
    {
        DEFAULT = 1,
        TEL = 2,
        CNC = 3,
        EDU = 4,
        CMC = 5,
        FOREIGN = 6,

        anhui_tel = 101,
        anhui_cnc = 102,
        aomen = 103,
        beijing_tel = 104,
        beijing_cnc = 105,
        chongqing_tel = 106,
        chongqing_cnc = 107,
        fujian_tel = 108,
        fujian_cnc = 109,
        gansu_tel = 110,
        gansu_cnc = 111,
        guangdong_tel = 112,
        guangdong_cnc = 113,
        guangxi_tel = 114,
        guangxi_cnc = 115,
        guizhou_tel = 116,
        guizhou_cnc = 117,
        hainan_tel = 118,
        hainan_cnc = 119,
        hebei_tel = 120,
        hebei_cnc = 121,
        henan_tel = 122,
        henan_cnc = 123,
        heilongjiang_tel = 124,
        heilongjiang_cnc = 125,
        hubei_tel = 126,
        hubei_cnc = 127,
        hunan_tel = 128,
        hunan_cnc = 129,
        jilin_tel = 130,
        jilin_cnc = 131,
        jiangsu_tel = 132,
        jiangsu_cnc = 133,
        jiangxi_tel = 134,
        jiangxi_cnc = 135,
        liaoning_tel = 136,
        liaoning_cnc = 137,
        neimeng_tel = 138,
        neimeng_cnc = 139,
        ningxia_tel = 140,
        ningxia_cnc = 141,
        qinghai_tel = 142,
        qinghai_cnc = 143,
        shandong_tel = 144,
        shandong_cnc = 145,
        shanxi_tel = 146,
        shanxi_cnc = 147,
        shaanxi_tel = 148,
        shaanxi_cnc = 149,
        shanghai_tel = 150,
        shanghai_cnc = 151,
        sichuan_tel = 152,
        sichuan_cnc = 153,
        taiwan = 154,
        tianjin_tel = 155,
        tianjin_cnc = 156,
        xizang_tel = 157,
        xizang_cnc = 158,
        xianggang = 159,
        xinjiang_tel = 160,
        xinjiang_cnc = 161,
        yunnan_tel = 162,
        yunnan_cnc = 163,
        zhejiang_tel = 164,
        zhejiang_cnc = 165,
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
    public enum DNSPodUnitValidate
    {
        FromDNSPod,
        Invalid,
        New,
    }
}