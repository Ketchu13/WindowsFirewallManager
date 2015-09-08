Imports NetFwTypeLib

Public Class Form2




    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        Dim CurrentProfiles
        Dim InterfaceArray
        Dim LowerBound
        Dim UpperBound
        Dim iterate
        Dim rule

        ' Profile Type
        Const NET_FW_PROFILE2_DOMAIN = 1
        Const NET_FW_PROFILE2_PRIVATE = 2
        Const NET_FW_PROFILE2_PUBLIC = 4

        ' Protocol
        Const NET_FW_IP_PROTOCOL_TCP = 6
        Const NET_FW_IP_PROTOCOL_UDP = 17
        Const NET_FW_IP_PROTOCOL_ICMPv4 = 1
        Const NET_FW_IP_PROTOCOL_ICMPv6 = 58

        ' Direction
        Const NET_FW_RULE_DIR_IN = 1
        Const NET_FW_RULE_DIR_OUT = 2

        ' Action
        Const NET_FW_ACTION_BLOCK = 0
        Const NET_FW_ACTION_ALLOW = 1


        ' Create the FwPolicy2 object.
        Dim fwPolicy2
        fwPolicy2 = CreateObject("HNetCfg.FwPolicy2")

        CurrentProfiles = fwPolicy2.CurrentProfileTypes

        '// The returned 'CurrentProfiles' bitmask can have more than 1 bit set if multiple profiles 
        '//   are active or current at the same time

        If (CurrentProfiles And NET_FW_PROFILE2_DOMAIN) Then
            Console.WriteLine("Domain Firewall Profile is active")
        End If

        If (CurrentProfiles And NET_FW_PROFILE2_PRIVATE) Then
            Console.WriteLine("Private Firewall Profile is active")
        End If

        If (CurrentProfiles And NET_FW_PROFILE2_PUBLIC) Then
            Console.WriteLine("Public Firewall Profile is active")
        End If

        ' Get the Rules object
        Dim RulesObject
        RulesObject = fwPolicy2.Rules

        ' Print all the rules in currently active firewall profiles.
        Console.WriteLine("Rules:")

        For Each rule In RulesObject
            If rule.Profiles And CurrentProfiles Then
                Console.WriteLine("  Rule Name:          " & rule.Name)
                Console.WriteLine("   ----------------------------------------------")
                Console.WriteLine("  Description:        " & rule.Description)
                Console.WriteLine("  Application Name:   " & rule.ApplicationName)
                Console.WriteLine("  Service Name:       " & rule.ServiceName)
                Select Case rule.Protocol
                    Case NET_FW_IP_PROTOCOL_TCP
                        Console.WriteLine("  IP Protocol:        TCP.")
                    Case NET_FW_IP_PROTOCOL_UDP
                        Console.WriteLine("  IP Protocol:        UDP.")
                    Case NET_FW_IP_PROTOCOL_ICMPv4
                        Console.WriteLine("  IP Protocol:        UDP.")
                    Case NET_FW_IP_PROTOCOL_ICMPv6
                        Console.WriteLine("  IP Protocol:        UDP.")
                    Case Else
                        Console.WriteLine("  IP Protocol:        " & rule.Protocol)
                End Select
                If rule.Protocol = NET_FW_IP_PROTOCOL_TCP Or rule.Protocol = NET_FW_IP_PROTOCOL_UDP Then
                    Console.WriteLine("  Local Ports:        " & rule.LocalPorts)
                    Console.WriteLine("  Remote Ports:       " & rule.RemotePorts)
                    Console.WriteLine("  LocalAddresses:     " & rule.LocalAddresses)
                    Console.WriteLine("  RemoteAddresses:    " & rule.RemoteAddresses)
                End If
                If rule.Protocol = NET_FW_IP_PROTOCOL_ICMPv4 Or rule.Protocol = NET_FW_IP_PROTOCOL_ICMPv6 Then
                    Console.WriteLine("  ICMP Type and Code:    " & rule.IcmpTypesAndCodes)
                End If
                Select Case rule.Direction
                    Case NET_FW_RULE_DIR_IN
                        Console.WriteLine("  Direction:          In")
                    Case NET_FW_RULE_DIR_OUT
                        Console.WriteLine("  Direction:          Out")
                End Select
                Console.WriteLine("  Enabled:            " & rule.Enabled)
                Console.WriteLine("  Edge:               " & rule.EdgeTraversal)
                Select Case rule.Action
                    Case NET_FW_ACTION_ALLOW
                        Console.WriteLine("  Action:             Allow")
                    Case NET_FW_ACTION_BLOCK
                        Console.WriteLine("  Action:             Block")
                End Select
                Console.WriteLine("  Grouping:           " & rule.Grouping)
                Console.WriteLine("  Edge:               " & rule.EdgeTraversal)
                Console.WriteLine("  Interface Types:    " & rule.InterfaceTypes)
                InterfaceArray = rule.Interfaces

                If String.IsNullOrEmpty(InterfaceArray) Then
                    Console.WriteLine("  Interfaces:         All")
                Else
                    LowerBound = LBound(InterfaceArray)
                    UpperBound = UBound(InterfaceArray)
                    Console.WriteLine("  Interfaces:     ")
                    For iterate = LowerBound To UpperBound
                        Console.WriteLine("       " & InterfaceArray(iterate))
                    Next

                End If

                Console.WriteLine("")
            End If
        Next
    End Sub
End Class