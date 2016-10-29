#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedClient;
using System.Text.RegularExpressions;
using System.ComponentModel;

#endregion

namespace ManagedClient
{
    /// <summary>
    /// Параметры и операции, связанные с поисковыми сценариями
    /// </summary>
    [Serializable]
    public sealed class IrbisSearchEngine
    {
        #region Nested classes
        [Serializable]
        public class SearchRequest
        {
            public String Text;
            public SearchOperand SearchStuff;
        }

        [Serializable]
        public class SearchTerm
        {
            public String Key;
            public String Name;
            public bool IsPresent;
            public bool Trunc;
            public List<PostingsParams> Postings;
            public String[] Fields;
            public String UserText;

            public SearchTerm()
            {
            }

            public SearchTerm(String termKey)
            {
                int index = termKey.IndexOf('=') + 1;
                if (termKey.EndsWith("$"))
                {
                    Trunc = true;
                    termKey = termKey.Remove(termKey.Length - 1);
                }
                else
                    Trunc = false;

                Key = termKey.Substring(0, index);
                Name = termKey.Substring(index);
                IsPresent = true;
            }
        }

        [Serializable]
        public struct PostingsParams
        {
            public String Text;
            public List<int> Mfn;
        }

        [Serializable]
        public class SearchOperand
        {
            public List<SearchTerm> SearchTerms;
            public String UserText;

            public SearchOperand()
            {
                SearchTerms = new List<SearchTerm>();
            }

            public void Add(SearchOperand op2)
            {
                SearchTerms.AddRange(op2.SearchTerms);
            }

            public void Add(SearchOperand op2, string oper_sign)
            {
                if (oper_sign == "^")
                    for (int i = 0; i < op2.SearchTerms.Count; i++)
                        op2.SearchTerms[i].IsPresent = !op2.SearchTerms[i].IsPresent;
                this.Add(op2);
                this.UserText = String.Format("{0}{1}{2}", UserText, UserWordsForOperators.ContainsKey(oper_sign) ? UserWordsForOperators[oper_sign] : oper_sign, op2.UserText);
            }
        }

        [Serializable]        
        public struct SearchScenario
        {
            public String ItemName;            

            public String Name
            {
                get
                {
                    return ItemName;
                }                
            }
            public String ItemPref;
            public DictionType ItemDictionType;
            public String ItemMenu;
            public String ItemF8For;
            public String ItemModByDic;
            public bool ItemTranc;
            public String ItemHint;
            public String ItemModByDicAuto;
            public LogicType ItemLogic;
            public String ItemAdv;
            public String ItemPft;
        }

        public enum DictionType { None, Explanation, Special }

        public enum LogicType { Or, OrAnd, OrAndNot, OrAndNotAndField, OrAndNotAndPhrase }

        [Serializable]
        public struct SearchQualifier
        {
            public String QualifName;
            public String QualifValue;
        }

        [Serializable]
        public class PartRequest
        {
            public string TermValue { get; set; }
            public int PrefixIndex { get; set; }
            public string Prefix
            {
                get
                {
                    return SearchRef.SearchScenarios [this.PrefixIndex].ItemPref;
                }
            }
            public string PrefixName
            {
                get
                {
                    return SearchRef.SearchScenarios[this.PrefixIndex].ItemName;
                }
            }

            public string Trunc
            {
                get
                {
                    return IrbisSearchEngine.TruncList[1 - Convert.ToByte(this.IsTrunc)];
                }
            }
            public string Logic
            {
                get
                {
                    return IrbisSearchEngine.UserWordsForOperators[IrbisSearchEngine.OperatorsList[this.LogicIndex]].Trim();
                }
            }
            public bool IsTrunc { get; set; } /*{ get { return Trunc == 0 ? "ДА" : "НЕТ"; }*/
            public int LogicIndex { get; set; }
            public string Qualifier { get; set; }
            public bool IsComplex;
            private ComplexSearch SearchRef;
            //public static List<String> Prefixes;

            public PartRequest(string TermValue, int PrefixIndex, bool IsTrunc, int LogicIndex, string Qualifier, ComplexSearch SearchRef)
            {
                this.TermValue = TermValue;
                //this.Prefix = prefixKey;
                this.PrefixIndex = PrefixIndex;
                this.IsTrunc = IsTrunc;
                this.LogicIndex = LogicIndex;
                this.Qualifier = Qualifier;
                this.SearchRef = SearchRef;
            }

            public PartRequest(string Term, int PrefixIndex, ComplexSearch SearchRef)                
            {
                TermValue = Term;
                this.PrefixIndex = PrefixIndex;
                IsTrunc = true;
                this.SearchRef = SearchRef;
            }


            public PartRequest(int index, ComplexSearch SearchRef)
            {
                this.IsComplex = true;
                TermValue = "#" + index;
                this.SearchRef = SearchRef;
            }

            /*public PartRequest()
            {
            }*/

            public PartRequest Copy()
            {
                PartRequest request;
                if (this.IsComplex)
                {
                    request = new PartRequest(0, this.SearchRef);
                    request.TermValue = this.TermValue;
                }
                else
                    request = new PartRequest(TermValue, PrefixIndex, IsTrunc, LogicIndex, Qualifier, SearchRef);
                return request;
            }
        }

        [Serializable]
        public class ComplexSearch
        {
            public BindingList<PartRequest> ComplexRequest;
            public List<SearchScenario> SearchScenarios;
            public IrbisSearchEngine SearchEngine;            

            public ComplexSearch(IrbisSearchEngine SearchEngine, int[] PrefixIndices)
            {
                this.SearchEngine = SearchEngine;

                if (PrefixIndices == null || PrefixIndices.Length == 0)                    
                {
                    PrefixIndices = new int[SearchEngine.SearchScenarios.Length];
                    for (int index = 0; index < SearchEngine.SearchScenarios.Length; index++)
                        PrefixIndices[index] = index;
                }

                SearchScenarios = new List<SearchScenario>();
                foreach (int prefixIndex in PrefixIndices)                
                    SearchScenarios.Add(SearchEngine.SearchScenarios[prefixIndex]);
                    

                ComplexRequest = new BindingList<PartRequest>();
                if (PrefixIndices.Length != 0)
                {
                    PartRequest request = new PartRequest("", 0, this);
                    ComplexRequest.Add(request);
                }
            }

            public void AddRequest(string Term, int PrefIndex)
            {
                ComplexRequest.Add(new PartRequest(Term, PrefIndex, this));
            }

            public void AddRequest(string Term, int PrefIndex, bool IsTrunc, int LogicIndex, string Qualifier)
            {
                ComplexRequest.Add(new PartRequest(Term, PrefIndex, IsTrunc, LogicIndex, Qualifier, this));
            }

            public void AddComplexRequest(int RequestIndex)
            {
                ComplexRequest.Add(new PartRequest(RequestIndex, this));
            }

            public void RemoveRequest(int index)
            {
                ComplexRequest.RemoveAt(index);
            }

            public PartRequest this[int index]
            {
                get
                {
                    return ComplexRequest[index];
                }
                set
                {
                    ComplexRequest[index] = value;
                }
            }

            public int Length
            {
                get
                {
                    return ComplexRequest.Count;
                }                
            }

            public PartRequest[] ToArray()
            {
                return ComplexRequest.ToArray();
            }

            public BindingList<PartRequest> Copy()
            {
                PartRequest[] resultRequests = new PartRequest[ComplexRequest.Count];
                for (int i = 0; i < resultRequests.Length; i++)
                    resultRequests[i] = ComplexRequest[i].Copy();

                return new BindingList<PartRequest>(resultRequests.ToList<PartRequest>());
            }

            public string GetRequestString(int index)
            {
                if (!this[index].IsComplex)
                    return String.Format("\"{0}{1}{2}\"{3}", this[index].Prefix, this[index].TermValue, this[index].IsTrunc ? "$" : "", this[index].Qualifier);
                else
                    return this[index].TermValue;
            }

            public string FormFullRequest()
            {
                bool UsePrevRequests = false;
                StringBuilder sb_exp = new StringBuilder();
                for (int index = 0; index < ComplexRequest.Count; index++)
                {
                    sb_exp.Append(GetRequestString(index));
                    if (this[index].IsComplex && !UsePrevRequests)
                        UsePrevRequests = true;                    

                    if (index != ComplexRequest.Count - 1)
                        sb_exp.Append(IrbisSearchEngine.OperatorsList [this[index].LogicIndex]);                          
                }

                string str = sb_exp.ToString();
                if (UsePrevRequests)
                    str = SearchEngine.AddComplexRequest(sb_exp.ToString());

                return str;
            }            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Минимальное необходимое количество символов для выделения термина
        /// </summary>

        public int MinLKWLight { get; set; }

        public SearchRequest CurrentRequest { get; private set; }

        public bool ProcessTerms { get; set; }


        #endregion

        #region Construction

        public IrbisSearchEngine(ManagedClient64 client)
        {
            IrbisSearchEngine.client = client;
            GetSearchScenarios();
            SearchRequests = new List<SearchRequest>();
            CurrentRequest = null;
            ProcessTerms = true;
            UserWordsForOperators = new Dictionary<string, string>();
            UserWordsForOperators["*"] = UserWordsForOperators["G "] = UserWordsForOperators["F "] = UserWordsForOperators[". "] = " И ";            
            UserWordsForOperators["+"] = " ИЛИ ";
            UserWordsForOperators["^"] = " НЕ ";
            OperatorsList = new string[] { "*", "+", "^" };
            TruncList = new string[] { "Да", "Нет" };
        }

        #endregion

        #region Private members

        static ManagedClient64 client;
        private SearchRequest searchRequest;        
        private int pos;       


        SearchOperand ParseOrExpression()
        {
            SearchOperand op1 = ParseAndExpression();
            SearchOperand op2;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == ' ') pos++;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == '+')
            {
                pos++;
                op2 = ParseAndExpression();
                op1.Add(op2, "+");
            }
            return op1;
        }

        SearchOperand ParseAndExpression()
        {
            SearchOperand op1 = ParseAndGExpression();
            SearchOperand op2;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == ' ') pos++;
            while (pos != searchRequest.Text.Length && (searchRequest.Text[pos] == '*' || searchRequest.Text[pos] == '^'))
            {
                switch (searchRequest.Text[pos])
                {
                    case '*':
                        pos++;
                        op2 = ParseAndGExpression();
                        op1.Add(op2, "*");
                        break;
                    case '^':
                        pos++;
                        op2 = ParseAndGExpression();
                        op1.Add(op2, "^");
                        break;
                }
            }
            return op1;
        }

        SearchOperand ParseAndGExpression()
        {
            SearchOperand op1 = ParseAndFExpression();
            SearchOperand op2;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == ' ') pos++;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == 'G')
            {
                pos++;
                if (pos == searchRequest.Text.Length || searchRequest.Text[pos] != ' ')
                    throw new RequestParsingException();
                op2 = ParseAndFExpression();
                op1.Add(op2, "G ");
            }
            return op1;
        }

        SearchOperand ParseAndFExpression()
        {
            SearchOperand op1 = ParseAnd_Expression();
            SearchOperand op2;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == ' ') pos++;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == 'F')
            {
                pos++;
                if (pos == searchRequest.Text.Length || searchRequest.Text[pos] != ' ')
                    throw new RequestParsingException();
                op2 = ParseAnd_Expression();
                op1.Add(op2, "F ");                
            }
            return op1;
        }

        SearchOperand ParseAnd_Expression()
        {
            SearchOperand op1 = ParseTerm();
            SearchOperand op2;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == ' ') pos++;
            while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == '.')
            {
                pos++;
                if (pos == searchRequest.Text.Length || searchRequest.Text[pos] != ' ')
                    throw new RequestParsingException();
                op2 = ParseTerm();                
                op1.Add(op2, ". ");
            }
            return op1;
        }

        SearchOperand ParseTerm()
        {
            while (searchRequest.Text[pos] == ' ') pos++;
            if (searchRequest.Text[pos] == '#')
            {                
                int pos0 = ++pos;
                while (pos != searchRequest.Text.Length && searchRequest.Text[pos] >= '0' && searchRequest.Text[pos] <= '9') pos++;
                int index = Convert.ToInt32(searchRequest.Text.Substring(pos0, pos - pos0));
                if (index < 1 || index > SearchRequests.Count)
                    throw new RequestParsingException();

                searchRequest.Text = searchRequest.Text.Substring(0, pos0 - 1) + "(" + SearchRequests[index - 1].Text + ")" + searchRequest.Text.Substring(pos);
                pos = pos0 - 1;
            }
            switch (searchRequest.Text[pos])
            {
                case '\"':
                    return ParseSimpleRequest();
                case '(':
                    pos++;
                    SearchOperand searchTerms = ParseOrExpression();
                    if (pos >= searchRequest.Text.Length || searchRequest.Text[pos] != ')')
                        throw new RequestParsingException();
                    pos++;
                    while (pos != searchRequest.Text.Length && searchRequest.Text[pos] == ' ') pos++;
                    searchTerms.UserText = "(" + searchTerms.UserText + ")";
                    return searchTerms;
                default:
                    throw new RequestParsingException();
            }
        }

        SearchOperand ParseSimpleRequest()
        {
            while (++pos < searchRequest.Text.Length && searchRequest.Text[pos] == ' ') ;
            int pos0 = pos;
            int length = 0;
            while (pos < searchRequest.Text.Length && searchRequest.Text[pos] != '\"')
            {
                length++;
                pos++;
            }

            if (pos == searchRequest.Text.Length)
                throw new RequestParsingException();

            String termKey = searchRequest.Text.Substring(pos0, length);
            SearchOperand op = new SearchOperand();            
            SearchTerm searchTerm = new SearchTerm(termKey);

            pos++;
            while (pos < searchRequest.Text.Length && searchRequest.Text[pos] == ' ') pos++;
            if (pos < searchRequest.Text.Length - 1 && searchRequest.Text[pos] == '/' && searchRequest.Text[pos + 1] == '(')
            {
                pos++;
                pos0 = ++pos;
                length = 0;

                while (pos < searchRequest.Text.Length && searchRequest.Text[pos] != ')')
                {
                    length++;
                    pos++;
                }

                if (pos >= searchRequest.Text.Length)
                    throw new RequestParsingException();

                searchTerm.Fields = searchRequest.Text.Substring(pos0, length).Split(',');

                while (++pos != searchRequest.Text.Length && searchRequest.Text[pos] == ' ') ;
            }
            else
                searchTerm.Fields = new string[0];

            op.UserText = null;
            foreach (SearchScenario scenario in SearchScenarios)
                if (scenario.ItemPref == searchTerm.Key)
                {
                    op.UserText = String.Format(UserTextMask, searchTerm.Name, scenario.ItemName);
                    break;
                }

            searchTerm.Name = searchTerm.Name.ToUpper();            
            op.SearchTerms.Add(searchTerm);
            return op;
        }

        #endregion

        #region Public methods

        public void ParseAddRequest(string Request)
        {
            searchRequest = null;

            foreach (SearchRequest request in SearchRequests)
                if (request.Text == Request)
                {
                    searchRequest = request;
                    CurrentRequest = searchRequest;
                }

            if (searchRequest == null)
            {
                searchRequest = new SearchRequest();
                searchRequest.Text = Request;
            }

            pos = 0;
            try
            {
                searchRequest.SearchStuff = ParseOrExpression();                
            }
            catch (RequestParsingException)
            {
                searchRequest.SearchStuff.SearchTerms = new List<SearchTerm>();
                throw new RequestParsingException();                
            }

            if (ProcessTerms)
                foreach (SearchTerm term in searchRequest.SearchStuff.SearchTerms)
                {
                    if (term.IsPresent)
                    {
                        term.Postings = new List<PostingsParams>();
                        if (String.IsNullOrEmpty(term.Name))
                            continue;

                        SearchTermInfo[] searchTermInfo = client.GetSearchTerms(term.Key + term.Name, 0);
                        foreach (SearchTermInfo term2 in searchTermInfo)
                            if (term.Trunc && term2.Text.StartsWith(term.Key + term.Name, StringComparison.CurrentCultureIgnoreCase) || term2.Text.Equals(term.Key + term.Name, StringComparison.CurrentCultureIgnoreCase))
                            {
                                PostingsParams Posting;
                                Posting.Text = term2.Text.Substring(term.Key.Length);

                                SearchPostingInfo[] searchPostingInfo = client.GetSearchPostings(term2.Text, 0, /*i + */1);
                                Posting.Mfn = new List<int>();

                                foreach (SearchPostingInfo posting in searchPostingInfo)
                                    if (!Posting.Mfn.Contains(posting.Mfn) && (term.Fields.Length == 0 || term.Fields.Contains(posting.Tag)))
                                        Posting.Mfn.Add(posting.Mfn);

                                term.Postings.Add(Posting);
                            }
                    }
                    term.Fields = null;
                }

            if (CurrentRequest != searchRequest)           
            {
                SearchRequests.Add(searchRequest);
                CurrentRequest = searchRequest;
            }
        }

        public string AddComplexRequest(string Request)
        {
            searchRequest = new SearchRequest();
            searchRequest.Text = Request;
            pos = 0;
            
            try
            {
                searchRequest.SearchStuff = ParseOrExpression();
            }
            catch (RequestParsingException)
            {
                searchRequest.SearchStuff.SearchTerms = new List<SearchTerm>();
                throw new RequestParsingException();
            }

            SearchRequests.Add(searchRequest);            
            return searchRequest.Text;
        }

        public void RemoveRequest(int index)
        {
            if (index >= 0 && index < SearchRequests.Count)
                SearchRequests.RemoveAt(index);
        }

        public void SetCurrentRequest(int index)
        {
            if (index >= 0 && index < SearchRequests.Count)
                CurrentRequest = SearchRequests[index];
        }

        public ComplexSearch CreateComplexSearch(int[] PrefixIndices)
        {
            return new ComplexSearch(this, PrefixIndices);
        }

        public string MarkEntries(String str, String word)
        {
            int index = 0;
            StringBuilder sb = new StringBuilder();
            int index0 = index;
            while (index < str.Length)
            {
                index = str.IndexOf(word, index0, StringComparison.CurrentCultureIgnoreCase);
                if (index < 8 || str.Substring(index - 8, 8) != "{select}")
                {
                    if (index == -1)
                        break;
                    sb.Append(str.Substring(index0, index - index0)).Append("{select}").Append(str.Substring(index, word.Length)).Append("{/select}");
                }
                else
                    sb.Append(str.Substring(index0, index - index0)).Append(str.Substring(index, word.Length));

                index += word.Length;
                index0 = index;
            }

            sb.Append(str.Substring(index0, str.Length - index0));

            return sb.ToString();
        }

        public void GetSearchScenarios()
        {
            string configuration = client.ReadTextFile(IrbisPath.System, "irbis_server.ini");
            IniFile iniFile = IniFile.ParseText<IniFile>(configuration);
            String IniFileName = String.Format("{0}{1}\\{1}.INI", iniFile.Get<String>("MAIN", "DataPath", "C:\\IRBIS64\\DATAI\\"), client.Database);
            if (System.IO.File.Exists(IniFileName))
                iniFile = IniFile.ParseFile<IniFile>(IniFileName, Encoding.Default);
            else
                iniFile = IniFile.ParseText<IniFile>(client.Configuration);
            
            IniFile.Section SearchSection = iniFile.GetSection("SEARCH");

            int itemCount = (SearchSection == null)
                ? 0
                : SearchSection.Get("ItemNumb", 0);

            SearchScenarios = new SearchScenario[itemCount];
            if (SearchSection != null)
            {
                for (int index = 0; index < SearchScenarios.Length; index++)
                {
                    SearchScenario searchScenario;
                    searchScenario.ItemName = SearchSection.Get("ItemName" + index);
                    searchScenario.ItemPref = SearchSection.Get("ItemPref" + index);
                    searchScenario.ItemDictionType = (DictionType)SearchSection.Get("ItemDictionType" + index, 0);
                    searchScenario.ItemMenu = SearchSection.Get("ItemMenu" + index);
                    searchScenario.ItemF8For = SearchSection.Get("ItemF8For" + index);
                    searchScenario.ItemModByDic = SearchSection.Get("ItemModByDic" + index);
                    try
                    {
                        searchScenario.ItemTranc = SearchSection.Get<int>("ItemTranc" + index, 0) != 0;
                    }
                    catch
                    {
                        searchScenario.ItemTranc = true;
                    }
                    searchScenario.ItemHint = SearchSection.Get("ItemHint" + index);
                    searchScenario.ItemModByDicAuto = SearchSection.Get("ItemModByDicAuto" + index);
                    try
                    {
                        searchScenario.ItemLogic = (LogicType)SearchSection.Get<int>("ItemLogic" + index, 0);
                    }
                    catch
                    {
                        searchScenario.ItemLogic = LogicType.Or;
                    }
                    searchScenario.ItemAdv = SearchSection.Get("ItemAdv" + index);
                    searchScenario.ItemPft = SearchSection.Get("ItemPft" + index);

                    SearchScenarios[index] = searchScenario;
                }


                SearchQualifiers = new SearchQualifier[SearchSection.Get("CvalifNumb", 0)];
                for (int index = 0; index < SearchQualifiers.Length; index++)
                {
                    SearchQualifier searchQualifier;
                    searchQualifier.QualifName = SearchSection.Get("CvalifName" + index);
                    searchQualifier.QualifValue = SearchSection.Get("CvalifValue" + index);
                    SearchQualifiers[index] = searchQualifier;
                }

                MinLKWLight = SearchSection.Get<int>("MinLKWLight");
            }
        }

        #endregion

        #region Object members

        public List<SearchRequest> SearchRequests;
        public SearchScenario[] SearchScenarios;
        public SearchQualifier[] SearchQualifiers;
        public static Dictionary<String, String> UserWordsForOperators;
        public static String[] OperatorsList;
        public static String[] TruncList;
        public static String UserTextMask = "\"{0}\" ({1})";

        #endregion
    }

    class RequestParsingException : ApplicationException
    {
        public RequestParsingException()
            : base()
        {
        }

        public RequestParsingException(string message)
            : base(message)
        {
        }

        public RequestParsingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}