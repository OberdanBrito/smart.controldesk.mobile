using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class RestService
{
    private HttpClient client;
    private readonly Uri origem = new Uri("http://anima.craos.net/ws/");
    private Dictionary<string, string> wsparamsquery = new Dictionary<string, string>
    {
        ["c"] = "7",
        ["cn"] = "as",
        ["process"] = "query"
    };


    public RestService()
    {
        client = new HttpClient();
    }

    public async Task<List<object>> QuerySelect(string From)
    {
        return await QuerySelect(From, null, null);
    }

    public async Task<List<object>> QuerySelect(string From, string Where)
    {
        return await QuerySelect(From, Where, null);
    }

    public async Task<List<object>> QuerySelect(string From, string Where, string Limit)
    {
    
        Dictionary<string, string> instrucao = new Dictionary<string, string>
        {
            ["command"] = "select",
            ["fields"] = "*",
            ["from"] = From
        };

        if (Where != null)
            instrucao["where"] = Where;

        if (Limit != null)
            instrucao["limit"] = Limit;

        wsparamsquery["params"] = JsonConvert.SerializeObject(instrucao);

        List<object> linhas = new List<object>();
        try
        {
            var resposta = await client.PostAsync(origem, new FormUrlEncodedContent(wsparamsquery));
            string dados = await resposta.Content.ReadAsStringAsync();
            List<lista> listavalores = JsonConvert.DeserializeObject<List<lista>>(dados);

            foreach (var item in listavalores)
            {
                linhas.Add(JsonConvert.DeserializeObject<object>(item.query));
            }

        }
        catch (Exception e)
        {

            Debug.WriteLine(e.Message);
            return null;
        }

        return linhas;
    }

    public async Task<List<object>> QueryInsert(string From, Dictionary<string, string> Fields, string Returning)
    {

        wsparamsquery["params"] = JsonConvert.SerializeObject(new instrucaoInsert
        {
            command = "insert",
            from = From,
            fields = Fields,
            returning = Returning
        });

        List<object> linhas = new List<object>();
        try
        {
            var resposta = await client.PostAsync(origem, new FormUrlEncodedContent(wsparamsquery));
            string dados = await resposta.Content.ReadAsStringAsync();
            List<lista> listavalores = JsonConvert.DeserializeObject<List<lista>>(dados);

            foreach (var item in listavalores)
            {
                linhas.Add(JsonConvert.DeserializeObject<object>(item.query));
            }

        }
        catch (Exception e)
        {

            Debug.WriteLine(e);
            return null;
        }

        return linhas;
    }

    public async Task<List<object>> QueryUpdate(string From, Dictionary<string, string> Fields, int Id, string Returning)
    {
    
        wsparamsquery["params"] = JsonConvert.SerializeObject(new instrucaoUpdate
        {
            command = "update",
            from = From,
            fields = Fields,
            where = $"id = {Id}",
            returning = Returning
        });

        List<object> linhas = new List<object>();
        try
        {
            var resposta = await client.PostAsync(origem, new FormUrlEncodedContent(wsparamsquery));
            string dados = await resposta.Content.ReadAsStringAsync();
            List<lista> listavalores = JsonConvert.DeserializeObject<List<lista>>(dados);

            foreach (var item in listavalores)
            {
                linhas.Add(JsonConvert.DeserializeObject<object>(item.query));
            }

        }
        catch (Exception e)
        {

            Debug.WriteLine(e);
            return null;
        }

        return linhas;
    }

    public async Task<List<object>> QueryDelete(string From, int Id, string Returning)
    {

        instrucaoDelete instrucao = new instrucaoDelete
        {
            command = "delete",
            from = From,
            where = $"id = {Id}",
            returning = Returning
        };

        wsparamsquery["params"] = JsonConvert.SerializeObject(instrucao);

        List<object> linhas = new List<object>();
        try
        {
            var resposta = await client.PostAsync(origem, new FormUrlEncodedContent(wsparamsquery));
            string dados = await resposta.Content.ReadAsStringAsync();

            if (dados == "false")
                return null;

            List<lista> listavalores = JsonConvert.DeserializeObject<List<lista>>(dados);

            foreach (var item in listavalores)
            {
                linhas.Add(JsonConvert.DeserializeObject<object>(item.query));
            }

        }
        catch (Exception e)
        {

            Debug.WriteLine(e);
            return null;
        }

        return linhas;
    }
}

public class lista
{
    public string query { get; set; }
}

public class instrucaoInsert
{
    public string command { get; set; }
    public string from { get; set; }
    public string returning { get; set; }
    public Dictionary<string, string> fields { get; set; }
}

public class instrucaoUpdate
{
    public string command { get; set; }
    public string from { get; set; }
    public string where { get; set; }
    public string returning { get; set; }
    public Dictionary<string, string> fields { get; set; }
}



public class instrucaoDelete
{
    public string command { get; set; }
    public string from { get; set; }
    public string where { get; set; }
    public string returning { get; set; }
}