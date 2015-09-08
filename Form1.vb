
Imports NetFwTypeLib
Imports System.Runtime.InteropServices

Public Class Form1
    Private Const CLSID_FIREWALL_MANAGER As String = "{304CE942-6E39-40D8-943A-B913C40C9CD4}"
    Private Const NET_FW_PROFILE_DOMAIN As NET_FW_PROFILE_TYPE_ = NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN

    Private Function GetCurrentProfile() As INetFwProfile
        Dim profile As INetFwProfile
        Try
            profile = GetFirewallManager().LocalPolicy.CurrentProfile
        Catch e As COMException
            Throw New NotSupportedException("Could not get the current profile (COMException)", e)
        Catch e As InvalidComObjectException
            Throw New NotSupportedException("Could not get the current profile (InvalidComObjectException)", e)
        End Try

        Return profile
    End Function

    Private Const CLSIDFireWallManager As String = "{304CE942-6E39-40D8-943A-B913C40C9CD4}"
    Private Function GetFirewallManager() As INetFwMgr
        Dim objectType As Type = Type.GetTypeFromCLSID(New Guid(CLSIDFireWallManager))
        Dim manager As INetFwMgr = TryCast(Activator.CreateInstance(objectType), INetFwMgr)
        If manager Is Nothing Then
            Throw New NotSupportedException("Could not load firewall manager")
        End If

        Return manager
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim CurrentProfiles


        ' Profile Type
        Const NET_FW_PROFILE2_DOMAIN = 1
        Const NET_FW_PROFILE2_PRIVATE = 2
        Const NET_FW_PROFILE2_PUBLIC = 4

        Dim fwPolicy2 As INetFwPolicy2


        Try
            fwPolicy2 = DirectCast(Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")), INetFwPolicy2)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        ' Create the FwPolicy2 object.



        CurrentProfiles = fwPolicy2.CurrentProfileTypes

        '// The returned 'CurrentProfiles' bitmask can
        '// have more than 1 bit set if multiple profiles 
        '// are active or current at the same time
        Try

        Catch ex As Exception
            If (CurrentProfiles And NET_FW_PROFILE2_DOMAIN) Then
                If fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_DOMAIN) = True Then
                    Console.WriteLine("Firewall is ON on domain profile.")
                Else
                    Console.WriteLine("Firewall is OFF on domain profile.")
                End If
            End If

            If (CurrentProfiles And NET_FW_PROFILE2_PRIVATE) Then
                If fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_PRIVATE) = True Then
                    Console.WriteLine("Firewall is ON on private profile.")
                Else
                    Console.WriteLine("Firewall is OFF on private profile.")
                End If
            End If

            If (CurrentProfiles And NET_FW_PROFILE2_PUBLIC) Then
                If fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_PUBLIC) = True Then
                    Console.WriteLine("Firewall is ON on public profile.")
                Else
                    Console.WriteLine("Firewall is OFF on public profile.")
                End If
            End If
        End Try

        '    Dim applications As INetFwAuthorizedApplications = GetCurrentProfile().AuthorizedApplications



    End Sub
    Public Function IsPortEnabled(portNumber As Integer, protocol As NET_FW_IP_PROTOCOL_) As Boolean
        ' Retrieve the open ports collection
        Dim openPorts As INetFwOpenPorts = GetCurrentProfile().GloballyOpenPorts
        If openPorts Is Nothing Then
            Return False
        End If

        ' Get the open port
        Try
            Dim openPort As INetFwOpenPort = openPorts.Item(portNumber, protocol)
            If openPort Is Nothing Then
                Return False
            End If
        Catch generatedExceptionName As System.IO.FileNotFoundException
            Return False
        End Try

        Return True
    End Function
    ''' <summary>
    ''' Returns a friendly string format of the policy type.
    ''' </summary>
    ''' <param name="profile">INetFwProfile object</param>
    ''' <returns>string</returns>
    Private Function GetPolicyType(profile As INetFwProfile) As String
        Dim policyType As String = String.Empty

        ' Displays what type of policy the Windows Firewall is controlled by.
        Select Case profile.Type
            Case NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN
                policyType = "Domain"
                Exit Select

            Case NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_STANDARD
                policyType = "Standard"
                Exit Select

            Case NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_CURRENT
                policyType = "Current"
                Exit Select

            Case NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_TYPE_MAX
                policyType = "Max"
                Exit Select

        End Select

        Return policyType
    End Function
    Private Sub DisplayFirewallProfile(manager As INetFwMgr)
        ListView1.Items.Clear()
        ListView2.Items.Clear()
        ListView3.Items.Clear()

        Dim profile As INetFwProfile = manager.LocalPolicy.CurrentProfile

        '             * Profile Information
        Label1.Text = "Firewall Policy Type: " & Me.GetPolicyType(profile)

        Label3.Text = "Exceptions Not Allowed: " & profile.ExceptionsNotAllowed
        Label4.Text = "Notifications Disabled: " & profile.NotificationsDisabled
        Label5.Text = "Remote Administration Enabled: " & profile.RemoteAdminSettings.Enabled

        Label7.Text = "Allow Inbound Echo Request: " & profile.IcmpSettings.AllowInboundEchoRequest
        Label8.Text = "Allow Inbound Mask Request: " & profile.IcmpSettings.AllowInboundMaskRequest
        Label9.Text = "Allow Inbound Router Request: " & profile.IcmpSettings.AllowInboundRouterRequest
        Label10.Text = "Allow Inbound TimeStamp Request: " & profile.IcmpSettings.AllowInboundTimestampRequest
        Label11.Text = "Allow Outbound Destination Unreachable: " & profile.IcmpSettings.AllowOutboundDestinationUnreachable
        '   Console.WriteLine("Allow Outbound Packet Too Big: " & profile.IcmpSettings.AllowOutboundPacketTooBig)
        '    Console.WriteLine("Allow Outbout Parameter Problem: " & profile.IcmpSettings.AllowOutboundParameterProblem)
        '   Console.WriteLine("Allow Outbound Source Quench: " & profile.IcmpSettings.AllowOutboundSourceQuench)
        '   Console.WriteLine("Allow Outbound Time Exceeded: " & profile.IcmpSettings.AllowOutboundTimeExceeded)
        '   Console.WriteLine("Allow Redirect: " & profile.IcmpSettings.AllowRedirect)
        Label2.Text = "Globally Opened Ports: " & profile.GloballyOpenPorts.Count
        Try


            ' Display detailed port information.
            For Each port As INetFwOpenPort In profile.GloballyOpenPorts

                Dim item As ListViewItem = Me.ListView3.Items.Add(port.Name)
                ''TODO add level for user in header
                Dim subitem As ListViewItem.ListViewSubItem = item.SubItems.Add(port.Port, Color.Red, Color.Black, Me.ListView1.Font)
                subitem = item.SubItems.Add(Me.GetPortType(port), Color.Red, Color.Black, Me.ListView1.Font)
                subitem = item.SubItems.Add(Me.GetIPVersion(port), Color.Red, Color.Black, Me.ListView1.Font)
                subitem = item.SubItems.Add(Me.GetPortType(port), Color.Red, Color.Black, Me.ListView1.Font)
                subitem = item.SubItems.Add(port.Enabled, Color.Red, Color.Black, Me.ListView1.Font)
                subitem = item.SubItems.Add(port.RemoteAddresses, Color.Red, Color.Black, Me.ListView1.Font)
            Next

        Catch ex As Exception

        End Try


        Label13.Text = "# of Services: " & profile.Services.Count
        Try
            ' Display detailed service information.
            For Each service As INetFwService In profile.Services
                ' Obtain all the port information the service is utilizing.
                For Each port As INetFwOpenPort In service.GloballyOpenPorts
                    Dim item As ListViewItem = Me.ListView2.Items.Add(service.Name)
                    ''TODO add level for user in header
                    Dim subitem As ListViewItem.ListViewSubItem = item.SubItems.Add(service.Enabled, Color.Red, Color.Black, Me.ListView1.Font)
                    subitem = item.SubItems.Add(Me.GetServiceScope(service), Color.Red, Color.Black, Me.ListView1.Font)
                    subitem = item.SubItems.Add(port.Port, Color.Red, Color.Black, Me.ListView1.Font)
                    subitem = item.SubItems.Add(Me.GetPortType(port), Color.Red, Color.Black, Me.ListView1.Font)
                    subitem = item.SubItems.Add(Me.GetIPVersion(port), Color.Red, Color.Black, Me.ListView1.Font)
                    subitem = item.SubItems.Add(port.Enabled, Color.Red, Color.Black, Me.ListView1.Font)
                    subitem = item.SubItems.Add(port.RemoteAddresses, Color.Red, Color.Black, Me.ListView1.Font)
                Next
            Next
        Catch ex As Exception

        End Try
        Try
            '             * Authorized Applications

            Label12.Text = "# of Authorized Applications: " & profile.AuthorizedApplications.Count

            ' Display detailed authorized application information.
            For Each application As INetFwAuthorizedApplication In profile.AuthorizedApplications

                Dim item As ListViewItem = Me.ListView1.Items.Add(application.Name)
                ''TODO add level for user in header
                Dim subitem As ListViewItem.ListViewSubItem = item.SubItems.Add(application.Enabled, Color.Red, Color.Black, Me.ListView1.Font)
                subitem = item.SubItems.Add(application.RemoteAddresses, Color.Red, Color.Black, Me.ListView1.Font)
                subitem = item.SubItems.Add(application.ProcessImageFileName, Color.Red, Color.Black, Me.ListView1.Font)
            Next
        Catch ex As Exception

        End Try


    End Sub
    Public Property IsWindowsFirewallOn() As Boolean
        Get
            Return GetCurrentProfile().FirewallEnabled
        End Get

        Set(value As Boolean)
            GetCurrentProfile().FirewallEnabled = value
        End Set
    End Property
    Private Function GetPortType(port As INetFwOpenPort) As String
        Dim protocolType As String = String.Empty

        Select Case port.Protocol
            Case NetFwTypeLib.NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP
                protocolType = "TCP"
                Exit Select

            Case NetFwTypeLib.NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP
                protocolType = "UDP"

                Exit Select
        End Select

        Return protocolType
    End Function

    ''' <summary>
    ''' Returns a friendly string format of the IP version.
    ''' </summary>
    ''' <param name="port">INetFwOpenPort port object</param>
    ''' <returns>string</returns>
    Private Function GetIPVersion(port As INetFwOpenPort) As String
        Dim ipVersion As String = String.Empty

        Select Case port.IpVersion
            Case NetFwTypeLib.NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY
                ipVersion = "Any"
                Exit Select

            Case NetFwTypeLib.NET_FW_IP_VERSION_.NET_FW_IP_VERSION_MAX
                ipVersion = "Max"
                Exit Select

            Case NetFwTypeLib.NET_FW_IP_VERSION_.NET_FW_IP_VERSION_V4
                ipVersion = "IPV4"
                Exit Select

            Case NetFwTypeLib.NET_FW_IP_VERSION_.NET_FW_IP_VERSION_V6
                ipVersion = "IPV6"
                Exit Select
        End Select

        Return ipVersion
    End Function

    ''' <summary>
    ''' Returns a friendly string format of the service scope.
    ''' </summary>
    ''' <param name="service">INetFwService object</param>
    ''' <returns>string</returns>
    Private Function GetServiceScope(service As INetFwService) As String
        Dim serviceScope As String = String.Empty

        Select Case service.Scope
            Case NetFwTypeLib.NET_FW_SCOPE_.NET_FW_SCOPE_ALL
                serviceScope = "All"
                Exit Select

            Case NetFwTypeLib.NET_FW_SCOPE_.NET_FW_SCOPE_CUSTOM
                serviceScope = "Custom"
                Exit Select

            Case NetFwTypeLib.NET_FW_SCOPE_.NET_FW_SCOPE_LOCAL_SUBNET
                serviceScope = "Local Subnet"
                Exit Select

            Case NetFwTypeLib.NET_FW_SCOPE_.NET_FW_SCOPE_MAX
                serviceScope = "Max"
                Exit Select
        End Select

        Return serviceScope
    End Function
    Public Function IsApplicationEnabled(applicationPath As String) As Boolean
        If [String].IsNullOrEmpty(applicationPath) Then
            Throw New ArgumentNullException("applicationPath")
        End If

        Try
            Dim application As INetFwAuthorizedApplication = GetCurrentProfile().AuthorizedApplications.Item(applicationPath)

            If application Is Nothing Then
                Return False
            End If
        Catch generatedExceptionName As System.IO.FileNotFoundException
            Return False
        End Try

        Return True
    End Function
    Private Const ProgramIDAuthorizedApplication As String = "HNetCfg.FwAuthorizedApplication"
    Public Function AddApplication(title As String, applicationPath As String, scope As NET_FW_SCOPE_, ipversion As NET_FW_IP_VERSION_) As Boolean
        If [String].IsNullOrEmpty(title) Then
            Throw New ArgumentNullException("title")
        End If

        If [String].IsNullOrEmpty(applicationPath) Then
            Throw New ArgumentNullException("applicationPath")
        End If

        If Not IsApplicationEnabled(applicationPath) Then
            ' Get the type based on program ID
            Dim type__1 As Type = Type.GetTypeFromProgID(ProgramIDAuthorizedApplication)
            Dim auth As INetFwAuthorizedApplication = TryCast(Activator.CreateInstance(type__1), INetFwAuthorizedApplication)

            auth.Name = title
            auth.ProcessImageFileName = applicationPath
            auth.Scope = scope
            auth.IpVersion = ipversion
            auth.Enabled = True

            Try
                GetCurrentProfile().AuthorizedApplications.Add(auth)
                DisplayFirewallProfile(GetFirewallManager())
            Catch generatedExceptionName As Exception
                Return False
            End Try
        End If

        Return True
    End Function
    Public Function RemoveApplication(title As String, applicationPath As String) As Boolean
        If [String].IsNullOrEmpty(title) Then
            Throw New ArgumentNullException("title")
        End If

        If [String].IsNullOrEmpty(applicationPath) Then
            Throw New ArgumentNullException("applicationPath")
        End If

        If IsApplicationEnabled(applicationPath) Then
            Try
                ' GetCurrentProfile().AuthorizedApplications.Remove(applicationPath)
                GetCurrentProfile().AuthorizedApplications.Item(applicationPath).Enabled = False
                '  GetCurrentProfile().
                'Remove(applicationPath)
                DisplayFirewallProfile(GetFirewallManager())
            Catch generatedExceptionName As Exception
                DisplayFirewallProfile(GetFirewallManager())
                Return False
            End Try
        End If

        Return True
    End Function

    Private Sub ListView1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListView1.MouseDoubleClick
        RemoveApplication(ListView1.SelectedItems(0).Text, ListView1.SelectedItems(0).SubItems(3).Text)
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DisplayFirewallProfile(GetFirewallManager())
    End Sub
End Class
