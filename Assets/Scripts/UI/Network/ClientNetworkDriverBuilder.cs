using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.TLS;
using Unity.Networking.Transport.Utilities;

public class ClientNetworkDriverBuilder : INetworkStreamDriverConstructor
{
    private static readonly string clientCertificate =
@"-----BEGIN CERTIFICATE-----
MIIDszCCApugAwIBAgIUCjTu4m1VFgkqVsHMUxiczT0nWtIwDQYJKoZIhvcNAQEL
BQAwaTELMAkGA1UEBhMCVUsxFjAUBgNVBAgMDVdlc3QgTWlkbGFuZHMxFDASBgNV
BAcMC1N0b3VyYnJpZGdlMRcwFQYDVQQKDA5BenVsb24gU3R1ZGlvczETMBEGA1UE
AwwKSG9BX1NlcnZlcjAeFw0yNDEwMTIxMDM1NDZaFw0yNzEwMTIxMDM1NDZaMGkx
CzAJBgNVBAYTAlVLMRYwFAYDVQQIDA1XZXN0IE1pZGxhbmRzMRQwEgYDVQQHDAtT
dG91cmJyaWRnZTEXMBUGA1UECgwOQXp1bG9uIFN0dWRpb3MxEzARBgNVBAMMCkhv
QV9TZXJ2ZXIwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDULBgBk7b3
vnQEPaPN9YsZYVaRHi4dH3mb2HA6k0H730N2a+OkXVAHqg+4QlGtJKuYjOSOuNjx
IMlI9z3cMcWUpi1gtX4QjGWeDVgadN5VEepqWq1tivba9DKqM2j1Pe7f6fRmUPbJ
RH4pkk9txCmvQILvmZkCbdHx6PGyLsQAkRJsxHldfEoUUIgrmZKBgp4MLng3p1im
+e/zD987OxXjn2iS6nx1N9RzXG7SOLHiZj8jr8a5pZPIbXltZ0vQeoheQc+zojxC
1MxbX01NQJ/5VCXYnOyFiWMfloj1zuBW5MRQdrKdw8bGzB85RVkAMXvorW89Lb+D
IiC1zpsqvPa9AgMBAAGjUzBRMB0GA1UdDgQWBBSg9GIAeWEmFN1qu5jYarE5I0mr
9DAfBgNVHSMEGDAWgBSg9GIAeWEmFN1qu5jYarE5I0mr9DAPBgNVHRMBAf8EBTAD
AQH/MA0GCSqGSIb3DQEBCwUAA4IBAQBhFZ1HAwZpMmyhnO2QGAQ6ja/XxOlXe/Ir
iundsqpYJXOTn19ERIqRbiTLhQcYEixLJuShY90s5IKa+B+Oz9aM8WAJnU8FHghy
SsCNCnE5v/GjuEH+Lsugz1H6BCxGeah082Lg/dWo+ekWZnt+ePprme725+YvMLy/
HUFBAgIG/QjQFCsothX0yPW7dJgwrQUGaZdf/a+qq8qGqzy6z7Cb4BBpsjY3rEaH
WD1FUZpGIURs+eko0lX5vV8V9uB/hkQejXHGPvigke6cOB+KwjGe4ICxxLer8DdC
BZn75FGJOVYrUwtlnD44Xq44p8qUorVKRxFguPZjbcuDgaAtJOXP
-----END CERTIFICATE-----";

    public void CreateDriver(UnityTransport transport, out NetworkDriver driver, out NetworkPipeline unreliableFragmentedPipeline, out NetworkPipeline unreliableSequencedFragmentedPipeline, out NetworkPipeline reliableSequencedPipeline)
    {
        NetworkSettings settings = new NetworkSettings();
        settings.WithNetworkConfigParameters();
        settings.WithFragmentationStageParameters();
        settings.WithReliableStageParameters();
        settings.WithSecureClientParameters(caCertificate: clientCertificate, serverName: "HoA_Server");
        driver = NetworkDriver.Create(settings);

        unreliableFragmentedPipeline = driver.CreatePipeline(typeof(FragmentationPipelineStage));
        unreliableSequencedFragmentedPipeline = driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));
        reliableSequencedPipeline = driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

        return;
    }
}
