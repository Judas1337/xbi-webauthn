﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fido2NetLib.AttestationFormat;
using Newtonsoft.Json;

namespace Fido2NetLib
{
    public class StaticMetadataRepository : IMetadataRepository
    {
        protected readonly IDictionary<Guid, MetadataTOCPayloadEntry> _entries;
        protected MetadataTOCPayload _toc;
        protected readonly HttpClient _httpClient;
        protected readonly DateTime? _cacheUntil;

        // from https://developers.yubico.com/U2F/yubico-u2f-ca-certs.txt
        protected const string YUBICO_ROOT = "MIIDHjCCAgagAwIBAgIEG0BT9zANBgkqhkiG9w0BAQsFADAuMSwwKgYDVQQDEyNZ" +
                                "dWJpY28gVTJGIFJvb3QgQ0EgU2VyaWFsIDQ1NzIwMDYzMTAgFw0xNDA4MDEwMDAw" +
                                "MDBaGA8yMDUwMDkwNDAwMDAwMFowLjEsMCoGA1UEAxMjWXViaWNvIFUyRiBSb290" +
                                "IENBIFNlcmlhbCA0NTcyMDA2MzEwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEK" +
                                "AoIBAQC/jwYuhBVlqaiYWEMsrWFisgJ+PtM91eSrpI4TK7U53mwCIawSDHy8vUmk" +
                                "5N2KAj9abvT9NP5SMS1hQi3usxoYGonXQgfO6ZXyUA9a+KAkqdFnBnlyugSeCOep" +
                                "8EdZFfsaRFtMjkwz5Gcz2Py4vIYvCdMHPtwaz0bVuzneueIEz6TnQjE63Rdt2zbw" +
                                "nebwTG5ZybeWSwbzy+BJ34ZHcUhPAY89yJQXuE0IzMZFcEBbPNRbWECRKgjq//qT" +
                                "9nmDOFVlSRCt2wiqPSzluwn+v+suQEBsUjTGMEd25tKXXTkNW21wIWbxeSyUoTXw" +
                                "LvGS6xlwQSgNpk2qXYwf8iXg7VWZAgMBAAGjQjBAMB0GA1UdDgQWBBQgIvz0bNGJ" +
                                "hjgpToksyKpP9xv9oDAPBgNVHRMECDAGAQH/AgEAMA4GA1UdDwEB/wQEAwIBBjAN" +
                                "BgkqhkiG9w0BAQsFAAOCAQEAjvjuOMDSa+JXFCLyBKsycXtBVZsJ4Ue3LbaEsPY4" +
                                "MYN/hIQ5ZM5p7EjfcnMG4CtYkNsfNHc0AhBLdq45rnT87q/6O3vUEtNMafbhU6kt" +
                                "hX7Y+9XFN9NpmYxr+ekVY5xOxi8h9JDIgoMP4VB1uS0aunL1IGqrNooL9mmFnL2k" +
                                "LVVee6/VR6C5+KSTCMCWppMuJIZII2v9o4dkoZ8Y7QRjQlLfYzd3qGtKbw7xaF1U" +
                                "sG/5xUb/Btwb2X2g4InpiB/yt/3CpQXpiWX/K4mBvUKiGn05ZsqeY1gx4g0xLBqc" +
                                "U9psmyPzK+Vsgw2jeRQ5JlKDyqE0hebfC1tvFu0CCrJFcw==";


        public StaticMetadataRepository(DateTime? cacheUntil = null)
        {
            _httpClient = new HttpClient();
            _entries = new Dictionary<Guid, MetadataTOCPayloadEntry>();
            _cacheUntil = cacheUntil;
        }

        public async Task<MetadataStatement> GetMetadataStatement(MetadataTOCPayloadEntry entry)
        {
            if (_toc == null)
                await GetToc();

            if (!string.IsNullOrEmpty(entry.AaGuid) && Guid.TryParse(entry.AaGuid, out Guid parsedAaGuid))
            {
                if (_entries.ContainsKey(parsedAaGuid))
                    return _entries[parsedAaGuid].MetadataStatement;
            }

            return null;
        }

        protected async Task<string> DownloadStringAsync(string url)
        {
            return await _httpClient.GetStringAsync(url);
        }


        public async Task<MetadataTOCPayload> GetToc()
        {
            var yubico = new MetadataTOCPayloadEntry
            {
                AaGuid = "f8a011f3-8c0a-4d15-8006-17111f9edc7d",
                Hash = "",
                StatusReports = new StatusReport[]
                {
                    new StatusReport() { Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED }
                },
                MetadataStatement = new MetadataStatement
                {
                    AttestationTypes = new ushort[]
                    {
                        (ushort)MetadataAttestationType.ATTESTATION_BASIC_FULL
                    },
                    Hash = "",
                    Description = "Yubico YubiKey FIDO2",
                    AttestationRootCertificates = new string[]
                    {
                        YUBICO_ROOT
                    }
                }
            };
            _entries.Add(new Guid(yubico.AaGuid), yubico);

            // YubiKey 5 USB and NFC AAGUID values from https://support.yubico.com/support/solutions/articles/15000014219-yubikey-5-series-technical-manual#AAGUID_Valuesxf002do
            var yubikey5usb = new MetadataTOCPayloadEntry
            {
                AaGuid = "cb69481e-8ff7-4039-93ec-0a2729a154a8",
                Hash = "",
                StatusReports = new StatusReport[]
                {
                    new StatusReport
                    {
                        Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED
                    }
                },
                MetadataStatement = new MetadataStatement
                {
                    AttestationTypes = new ushort[]
                    {
                        (ushort)MetadataAttestationType.ATTESTATION_BASIC_FULL
                    },
                    Hash = "",
                    Description = "Yubico YubiKey 5 USB",
                    AttestationRootCertificates = new string[]
                    {
                        YUBICO_ROOT
                    }
                }
            };
            _entries.Add(new Guid(yubikey5usb.AaGuid), yubikey5usb);

            var yubikey5nfc = new MetadataTOCPayloadEntry
            {
                AaGuid = "fa2b99dc-9e39-4257-8f92-4a30d23c4118",
                Hash = "",
                StatusReports = new StatusReport[]
                {
                    new StatusReport
                    {
                        Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED
                    }
                },
                MetadataStatement = new MetadataStatement
                {
                    AttestationTypes = new ushort[]
                    {
                        (ushort)MetadataAttestationType.ATTESTATION_BASIC_FULL
                    },
                    Hash = "",
                    Description = "Yubico YubiKey 5 NFC",
                    AttestationRootCertificates = new string[]
                    {
                        YUBICO_ROOT
                    }
                }
            };
            _entries.Add(new Guid(yubikey5nfc.AaGuid), yubikey5nfc);

            var yubicoSecuriyKeyNfc = new MetadataTOCPayloadEntry
            {
                AaGuid = "6d44ba9b-f6ec-2e49-b930-0c8fe920cb73",
                Hash = "",
                StatusReports = new StatusReport[] { new StatusReport() { Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED } },
                MetadataStatement = new MetadataStatement
                {
                    Description = "Yubico Security Key NFC",
                    Icon = "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4KPCEtLSBHZW5lcmF0b3I6IEFkb2JlIElsbHVzdHJhdG9yIDIzLjAuMSwgU1ZHIEV4cG9ydCBQbHVnLUluIC4gU1ZHIFZlcnNpb246IDYuMDAgQnVpbGQgMCkgIC0tPgo8c3ZnIHZlcnNpb249IjEuMSIgaWQ9Ill1YmljbyIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4bWxuczp4bGluaz0iaHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluayIgeD0iMHB4IiB5PSIwcHgiCgkgdmlld0JveD0iMCAwIDc2OCA3NjgiIHN0eWxlPSJlbmFibGUtYmFja2dyb3VuZDpuZXcgMCAwIDc2OCA3Njg7IiB4bWw6c3BhY2U9InByZXNlcnZlIj4KPHN0eWxlIHR5cGU9InRleHQvY3NzIj4KCS5zdDB7ZmlsbDojOUFDQTNDO30KPC9zdHlsZT4KPHBvbHlnb24gaWQ9IlkiIGNsYXNzPSJzdDAiIHBvaW50cz0iMjE4LjQzLDIxMS44MSAzMTYuNDksMjExLjgxIDM4Ni41Miw0MDAuMDcgNDUzLjIsMjExLjgxIDU0OS41NywyMTEuODEgMzg3LjA4LDYxMS44NiAKCTI4Ni4yMyw2MTEuODYgMzMyLjE3LDUwMi4wNCAiLz4KPHBhdGggaWQ9IkNpcmNsZV8xXyIgY2xhc3M9InN0MCIgZD0iTTM4NCwwQzE3MS45MiwwLDAsMTcxLjkyLDAsMzg0czE3MS45MiwzODQsMzg0LDM4NHMzODQtMTcxLjkyLDM4NC0zODRTNTk2LjA4LDAsMzg0LDB6CgkgTTM4NCw2OTMuNThDMjEzLjAyLDY5My41OCw3NC40Miw1NTQuOTgsNzQuNDIsMzg0UzIxMy4wMiw3NC40MiwzODQsNzQuNDJTNjkzLjU4LDIxMy4wMiw2OTMuNTgsMzg0UzU1NC45OCw2OTMuNTgsMzg0LDY5My41OHoiLz4KPC9zdmc+Cg==",
                    AttachmentHint = 6,
                    AttestationTypes = new ushort[] { (ushort)MetadataAttestationType.ATTESTATION_BASIC_FULL },
                    Hash = "",
                    AttestationRootCertificates = new string[]
                    {
                        YUBICO_ROOT
                    }
                }
            };
            _entries.Add(new Guid(yubicoSecuriyKeyNfc.AaGuid), yubicoSecuriyKeyNfc);

            var msftWhfbSoftware = new MetadataTOCPayloadEntry
            {
                AaGuid = "6028B017-B1D4-4C02-B4B3-AFCDAFC96BB2",
                Hash = "",
                StatusReports = new StatusReport[]
                {
                    new StatusReport
                    {
                        Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED
                    }
                },
                MetadataStatement = new MetadataStatement
                {
                    AttestationTypes = new ushort[]
                    {
                        (ushort)MetadataAttestationType.ATTESTATION_BASIC_FULL
                    },
                    Hash = "",
                    Description = "Windows Hello software authenticator"
                }
            };
            _entries.Add(new Guid(msftWhfbSoftware.AaGuid), msftWhfbSoftware);
            var msftWhfbSoftwareVbs = new MetadataTOCPayloadEntry
            {
                AaGuid = "6E96969E-A5CF-4AAD-9B56-305FE6C82795",
                Hash = "",
                StatusReports = new StatusReport[]
                {
                    new StatusReport
                    {
                        Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED
                    }
                },
                MetadataStatement = new MetadataStatement
                {
                    AttestationTypes = new ushort[]
                    {
                        (ushort)MetadataAttestationType.ATTESTATION_BASIC_FULL
                    },
                    Hash = "",
                    Description = "Windows Hello VBS software authenticator"
                }
            };
            _entries.Add(new Guid(msftWhfbSoftwareVbs.AaGuid), msftWhfbSoftwareVbs);
            var msftWhfbHardware = new MetadataTOCPayloadEntry
            {
                AaGuid = "08987058-CADC-4B81-B6E1-30DE50DCBE96",
                Hash = "",
                StatusReports = new StatusReport[]
                {
                    new StatusReport
                    {
                        Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED
                    }
                },
                MetadataStatement = new MetadataStatement
                {
                    AttestationTypes = new ushort[]
                    {
                        (ushort)MetadataAttestationType.ATTESTATION_BASIC_FULL
                    },
                    Hash = "",
                    Description = "Windows Hello hardware authenticator"
                }
            };
            _entries.Add(new Guid(msftWhfbHardware.AaGuid), msftWhfbHardware);
            var msftWhfbHardwareVbs = new MetadataTOCPayloadEntry
            {
                AaGuid = "9DDD1817-AF5A-4672-A2B9-3E3DD95000A9",
                Hash = "",
                StatusReports = new StatusReport[]
                {
                    new StatusReport
                    {
                        Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED
                    }
                },
                MetadataStatement = new MetadataStatement
                {
                    AttestationTypes = new ushort[]
                    {
                        (ushort)MetadataAttestationType.ATTESTATION_BASIC_FULL
                    },
                    Hash = "",
                    Description = "Windows Hello VBS hardware authenticator"
                }
            };
            _entries.Add(new Guid(msftWhfbHardwareVbs.AaGuid), msftWhfbHardwareVbs);

            var solostatement = await DownloadStringAsync("https://raw.githubusercontent.com/solokeys/solo/master/metadata/Solo-FIDO2-CTAP2-Authenticator.json");
            var soloMetadataStatement = JsonConvert.DeserializeObject<MetadataStatement>(solostatement);
            var soloKeysSolo = new MetadataTOCPayloadEntry
            {
                AaGuid = soloMetadataStatement.AaGuid,
                Url = "https://raw.githubusercontent.com/solokeys/solo/master/metadata/Solo-FIDO2-CTAP2-Authenticator.json",
                StatusReports = new StatusReport[]
                {
                    new StatusReport
                    {
                        Status = AuthenticatorStatus.NOT_FIDO_CERTIFIED
                    }
                },
                MetadataStatement = soloMetadataStatement
            };
            _entries.Add(new Guid(soloKeysSolo.AaGuid), soloKeysSolo);

            foreach (var entry in _entries)
            {
                entry.Value.MetadataStatement.AaGuid = entry.Value.AaGuid;
            }

            _toc = new MetadataTOCPayload()
            {
                Entries = _entries.Select(o => o.Value).ToArray(),
                NextUpdate = _cacheUntil?.ToString("yyyy-MM-dd") ?? "", //Results in no caching
                LegalHeader = "Static FAKE",
                Number = 1
            };

            return _toc;
        }
    }
}
