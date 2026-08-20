# Stage 0B .NET Boundary Remediation Approval

**Identifier:** GOV-052  
**Version:** 1.0  
**Status:** Approved  
**Decision Date:** 2026-07-26  
**Effective Date:** 2026-07-26  
**Decision Authority:** رائد عموره, Project Owner and current Falcon Constitutional Authority  
**Subject:** STG-0B-STOP-001 remediation and bounded Stage 0B continuation  
**Stage 0B Authority:** Resumed within GOV-051 and this remediation only  
**Activation Authority:** Not Granted  
**Stage 0C Authority:** Not Granted  
**Production Authority:** Not Granted  
**Cloud Deployment Authority:** Not Granted  
**Financial Authority:** Not Granted

## 1. Approval Declaration

> **موافق على الحل الآمن ومتابعة Stage 0B ضمن الحدود السابقة.**

## 2. Interpreted Decision

The Project Owner approves the remediation proposed by STG-0B-STOP-001:

- contain all .NET CLI state inside a repository-local ignored workspace;
- disable .NET first-run experience;
- disable .NET telemetry;
- disable automatic ASP.NET development-certificate generation;
- use a repository-local application-data path;
- use a repository-local package cache;
- clear all package sources;
- prohibit installation and download;
- prohibit network-dependent restore;
- disregard and never use the development certificate reported in the external Codex sandbox profile;
- and resume Stage 0B only within GOV-051.

## 3. External Certificate Boundary

The external development certificate:

- is not Falcon material;
- is not trusted;
- is not admitted;
- shall not be imported, referenced, used, or relied upon;
- and shall not be removed through a broad destructive cleanup command that could affect unrelated development certificates.

## 4. Stop Rule

Stage 0B shall stop again if:

- .NET attempts to read or write outside the declared isolated paths;
- a package source is required;
- installation or download is required;
- certificate generation recurs;
- network access is required;
- or any GOV-051 boundary is exceeded.

## 5. Preserved Non-Authorities

This decision grants no new candidate scope and does not authorize Activation, Stage 0C, general Falcon behavior, production, cloud deployment, financial connectivity, or financial activity.

## 6. Approval Record

| Role | Name | Decision | Date |
|---|---|---|---|
| Project Owner | رائد عموره | Approved safe remediation and Stage 0B continuation | 2026-07-26 |
| Falcon Constitutional Authority | رائد عموره | Preserved all GOV-051 boundaries | 2026-07-26 |
