# STG-0A-ENV-001 — Windows Preparation Environment Profile

**Identifier:** STG-0A-ENV-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-25  
**Approval Date:** 2026-07-26  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** STG-0A-PROP-001; ENV-001 v1.1  
**Approval Record:** GOV-048  
**Implementation Authority:** Not Granted  
**Activation Authority:** Not Granted  
**Cloud Deployment Authority:** Not Granted  
**Financial Authority:** Not Granted

## 1. Purpose

This document defines the intended local Windows preparation environment for Stage 0A.

It does not admit or activate the environment.

## 2. Platform Position

Windows is the first preparation platform.

Windows is temporary, local, and transitional.

Falcon remains cloud-ready and must preserve future portability to Linux and Oracle Cloud.

## 3. Environment Boundary

The preparation environment is limited to the Falcon local workspace and approved evidence paths.

Personal machine details are evidence, not Falcon policy.

## 4. Required Environment Facts

The environment profile SHALL record:

- operating system family and version;
- repository path;
- Git version;
- available .NET information, if already installed;
- available editor or shell information, if relevant;
- network status used for GitHub documentation push, if authorized;
- and evidence directory path.

## 5. Portability Rules

Stage 0A SHALL NOT create assumptions that prevent later Oracle Cloud migration.

It SHALL NOT hard-code personal paths into Falcon meaning.

It SHALL NOT allow Windows-specific behavior to cross Falcon Contracts.

## 6. Non-Activation Rule

This profile does not activate a Windows Falcon environment, Provider, Profile, Pipeline, runner, or operational runtime.
