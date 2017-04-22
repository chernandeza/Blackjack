<%@ Page Language="C#" Async="true" AutoEventWireup="true" Debug="true"  %>

<%@ Import Namespace="System.Threading.Tasks" %>

<%@ Import Namespace="System.Net" %>

<%@ Import Namespace="System.Net.Sockets" %>

<%@ Import Namespace="System.IO" %>

 

<script runat="server" language="C#">

 

  public void Page_Load(object sender, EventArgs e)

  {

    //RegisterAsyncTask(new PageAsyncTask(DoSendRequest));

    //RegisterAsyncTask(new PageAsyncTask(Button1_Click));

  }

 

  private async Task<string> SendRequest(string server, int port, string method, string data)

  {

    try

    {

      // determine IP address of server

      IPAddress ipAddress = null;

      IPHostEntry ipHostInfo = System.Net.Dns.GetHostEntry(server);

      for (int i = 0; i < ipHostInfo.AddressList.Length; ++i)

      {

        if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)

        {

          ipAddress = ipHostInfo.AddressList[i];

          break;

        }

      }

      if (ipAddress == null)

        throw new Exception("No IPv4 address for server");

 

      TcpClient client = new TcpClient();

      await client.ConnectAsync(ipAddress, port); // connect to the server

 

      NetworkStream networkStream = client.GetStream();

      StreamWriter writer = new StreamWriter(networkStream);

      StreamReader reader = new StreamReader(networkStream);

 

     writer.AutoFlush = true;

      string requestData = "method=" + method + "&" + "data=" + data + "&eor"; // 'end-of-requet'

      await writer.WriteLineAsync(requestData);

      string response = await reader.ReadLineAsync();

 

      client.Close();

 

      return response;

 

    }

    catch (Exception ex)

    {

      return ex.Message;

    }

  } // SendRequest

 

  private async void Button1_Click(object sender, System.EventArgs e)

  {

    try

    {

      string server = "VTE.redmond.corp.microsoft.com";

      int port = 50000;

      string method = TextBox1.Text;

      string data = TextBox2.Text;

 

      string sResponse = await SendRequest(server, port, method, data);

      double dResponse = double.Parse(sResponse);

      TextBox3.Text = dResponse.ToString("F2");

 

      

    }

    catch (Exception ex)

    {

      TextBox3.Text = ex.Message;

    }

    

  }

</script>

 

<head>

    <title>Demo</title>

</head>

 

<body>

    <form id="form1" runat="server">

    <div>

    

    <p>Enter service method: <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox></p>

    <p>Enter data: <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox></p>

    <p><asp:Button Text="Send Request" id="Button1" runat="server"  OnClick="Button1_Click"> </asp:Button> </p>

    <p>Response: <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox></p>

    <p>Dummy responsive control: <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox></p>

 

    </div>

    </form>

   

</body>

</html>

