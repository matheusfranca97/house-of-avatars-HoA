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

public class ServerNetworkDriverBuilder : INetworkStreamDriverConstructor
{
#if UNITY_SERVER
    private static readonly string serverCertificate =
@"-----BEGIN CERTIFICATE-----
MIIDWTCCAkECFAtfO6HpmlSfxX0Lo4ISDRHkqEEQMA0GCSqGSIb3DQEBCwUAMGkx
CzAJBgNVBAYTAlVLMRYwFAYDVQQIDA1XZXN0IE1pZGxhbmRzMRQwEgYDVQQHDAtT
dG91cmJyaWRnZTEXMBUGA1UECgwOQXp1bG9uIFN0dWRpb3MxEzARBgNVBAMMCkhv
QV9TZXJ2ZXIwHhcNMjQxMDEyMTAzNzM2WhcNMjUxMDEyMTAzNzM2WjBpMQswCQYD
VQQGEwJVSzEWMBQGA1UECAwNV2VzdCBNaWRsYW5kczEUMBIGA1UEBwwLU3RvdXJi
cmlkZ2UxFzAVBgNVBAoMDkF6dWxvbiBTdHVkaW9zMRMwEQYDVQQDDApIb0FfU2Vy
dmVyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwX6146UERCevjbaX
MZYHzvi13N+VYwwgMSpDgfda75LGjYNykwCEeu//hcJt+o/pdXKiOBY8gC8hgIpG
XRuoxRpH+PG3CZCA1J5toXmUC/M4bYfFlLp3okhpTMIxw3o7kXwHENDtVcsYkpWn
ItsPF6Q9B2/I+B/hK2TMMbFiXCEldSfVgA8h3diugxA8W8k0tYPm3mJnwzL49rpT
RdDKkE8R+6MdRjwMZH16SbgWK4PUuoXYietChnNbRSiWrXDavHdX3nTCJd2CErcp
DxY8RjAyX3q0B+TfjPOC78U7rQAOnYgsPIniGXd9dCSN+sFhFsq/yDSN+o+A5FXK
AOpN8QIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQBo9c8iyu2Tvbk42j5IV63l3wOU
C4RB1j9cesOeNuCPDzxf+TLrQAUQzlFCHDM5OobPsw1qrjRsYfwaHH6hDvol9H0b
pzgISzdxOnQlgKNZPsD+pdSxO6wLXprVrTt1Mw7u+IOIQe0c0j0WzQ5fdHOyUk0y
VP6mmwhHGKSVcKcvYZ9+DOoHWinHSQt1nlHM3IVsBriQ0LCwGKZs32mF4LjXACsD
giapQTaPR2d7S6khlDzexiC6Ny2JnGhH1+8C4PHfjaaVBP9YbEJJzwiH3mwPtvFL
h3mNU26GIH3GaJ5ph7rXy92bpzO3zLpST0VEw3EwbtnoIHNgjZZRQRuNCtP5
-----END CERTIFICATE-----";
    private static readonly string serverPrivateKey =
@"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAwX6146UERCevjbaXMZYHzvi13N+VYwwgMSpDgfda75LGjYNy
kwCEeu//hcJt+o/pdXKiOBY8gC8hgIpGXRuoxRpH+PG3CZCA1J5toXmUC/M4bYfF
lLp3okhpTMIxw3o7kXwHENDtVcsYkpWnItsPF6Q9B2/I+B/hK2TMMbFiXCEldSfV
gA8h3diugxA8W8k0tYPm3mJnwzL49rpTRdDKkE8R+6MdRjwMZH16SbgWK4PUuoXY
ietChnNbRSiWrXDavHdX3nTCJd2CErcpDxY8RjAyX3q0B+TfjPOC78U7rQAOnYgs
PIniGXd9dCSN+sFhFsq/yDSN+o+A5FXKAOpN8QIDAQABAoIBAGFZKpGZWAgiH0Sg
9HhSDyOmJXk2U6Y9V4Tkyon8tJeLtLFFzMMAo6ZmUJwvMb254a7hOZQWO+IR1D0j
VDtLyyE/E66/jWMWfHp8KpPu4vkQKPeSM2mcVswiujeQDBFY0ddkGvnu4zkisP4u
pKP4qiMu0jWHnAiZoWN/luv5Xo8SjE2NjsTgAKUq/nyJSFy4S7cpajf6VH9LNgYS
PeR//DyKg+BURRtKjcUT/19JkLO3pvxl9rguSq6CTZq1l4DG2lkJy+B956oPesKU
pQ8nqVYeHDmpySNH5glNq4HBkE6K//zrTeIB8bu58stOCCCRm+/0W8QBde0AMr5r
+XuPenECgYEA/MgV4FBxhQq41WdtGO/NjTUAItJy22LMejWwMeYbzlWlwpWLV7F8
Z3CdnHjAAkPQKRGcQdbK91G8E7rPNkzQYqcKY15gfOiK3L2bM56NLiCFIqXlzdWI
PXnRsfsc7gw1h50yEdC8VWsxBfXtHP4asIhgAMfAEBYEjaRssIwrFhsCgYEAw/Vi
+2jU4bMDvpbXPpSxdOJFtDJpIac+LGHBdUSEj+41puT1Sascro1Pyokgsy9I4HSq
9EmuUx6AtkfU/u8wjDWRcIXXhb6WV8yOxyqu8MjoAegOzk5OBrhTIL6kP/ZbitJo
tydi7x4M9hntn8TkFbIZSWMstHCvWb9QnKyMXOMCgYAKVErAjcj1vMhsv/svR61I
ld/ZjGvxFwpv+/2lLFf6iHlriBzXioMg3vMxz6VY8lhxNS0Da7mDfa2HyNxqxZzG
SzkbcmHS+NWjy4OqClKOjfmivtCzJoSYrn+pHC/Ecm9FiWDgZX0sqGKqcbAsvR1u
FUSHA6KPhbbN6ugeFrwz/QKBgElIcpzs5ngFl4fmJ1b7CqZYnJK4K4LvZZv5bvzp
A95DyoLAq07ClDZfGJD42WbJbyqp1ukGyQ/Cn4YLtQcl8nTs75gyJZiZ3uW01Ux8
lPHtYH6eBzN2K03uDwB26zwUaMWwzIJ6U1BzX4uFxMz0OAw5D6XXVfehEKKynnYJ
PXDbAoGBAJacZkPMh4roA8Stii7gnh4LqlqUfbnIux7CqHGYSprl9uSmBbicDEvz
5EoF9/2jEaq2cqRsYXrLexf3YkBlVo8sy+EvSVz8JhTkRgFgDF7p5VqK5pXBRRnz
NgVIknKY/55pBVa8s0YS8O1l0jbVPFsYOCtyNPkdiWdLCGZqXJ6R
-----END RSA PRIVATE KEY-----";
#endif
    public void CreateDriver(UnityTransport transport, out NetworkDriver driver, out NetworkPipeline unreliableFragmentedPipeline, out NetworkPipeline unreliableSequencedFragmentedPipeline, out NetworkPipeline reliableSequencedPipeline)
    {
#if UNITY_SERVER
        NetworkSettings settings = new NetworkSettings();
        settings.WithSecureServerParameters(certificate: serverCertificate, privateKey: serverPrivateKey);
        driver = NetworkDriver.Create(settings);

        unreliableFragmentedPipeline = driver.CreatePipeline(typeof(FragmentationPipelineStage));
        unreliableSequencedFragmentedPipeline = driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));
        reliableSequencedPipeline = driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

        return;
#endif
        throw new NotImplementedException();
    }
}
