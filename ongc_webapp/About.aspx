<%@ Page Title="About Us - ONGC" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="ongc_webapp.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* Tightened About Page Layout Engine */
        .about-page-wrapper {
            max-width: 1300px;
            margin: 15px auto 40px auto; 
            padding: 0 40px;
        }

        .about-main-title {
            color: #1a202c;
            font-weight: 800;
            font-size: 2.6rem;
            letter-spacing: -0.5px;
            margin-bottom: 20px;
            position: relative;
            padding-bottom: 12px;
        }

        .about-main-title::after {
            content: "";
            position: absolute;
            left: 0; bottom: 0;
            width: 60px; height: 4px;
            background-color: #7a0616;
            border-radius: 2px;
        }

        /* Hero Asset Placement directly below title */
        .about-asset-image {
            width: 100%;
            height: auto;
            max-height: 400px;
            object-fit: cover;
            border-radius: 8px;
            border: 1px solid #e2e8f0;
            margin-bottom: 35px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.06);
        }

        /* Core Prose Typography */
        .about-editorial-text {
            color: #2d3748;
            font-size: 1.1rem;
            line-height: 1.8;
            margin-bottom: 25px;
            text-align: justify;
        }

        .about-highlight-text {
            font-weight: 700;
            color: #7a0616;
        }

        /* Premium Statistics Sidebar Card Layout */
        .stats-sidebar-card {
            background-color: #fffdfa;
            border: 1px solid #f0e6db;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 4px 15px rgba(122, 6, 22, 0.02);
            position: sticky;
            top: 20px;
        }

        .sidebar-card-title {
            color: #7a0616;
            font-weight: 700;
            font-size: 1.2rem;
            margin-bottom: 20px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            border-bottom: 1px solid #e2e8f0;
            padding-bottom: 10px;
        }

        .stat-metric-row {
            margin-bottom: 18px;
        }

        .stat-metric-row:last-child {
            margin-bottom: 0;
        }

        .stat-metric-value {
            font-size: 1.75rem;
            font-weight: 800;
            color: #7a0616;
            line-height: 1.2;
        }

        .stat-metric-label {
            font-size: 0.9rem;
            color: #4a5568;
            font-weight: 500;
            margin-top: 2px;
        }
    </style>

    <div class="about-page-wrapper">
        <div class="row g-5">
            
            <div class="col-lg-8 pe-lg-5">
                <h1 class="about-main-title">About Us</h1>
                
                <img src="rig.jpg" alt="ONGC Offshore Production Rig" class="about-asset-image" />
                
                <p class="about-editorial-text">
                    Maharatna <span class="about-highlight-text">ONGC</span> is the largest producer of crude oil and natural gas in India, contributing around <span class="about-highlight-text">70 percent</span> of Indian domestic production. As India's premier energy anchor, ONGC features a completely integrated infrastructure matrix with unique in-house service capabilities across all operational horizons of Exploration and Production (E&P) of oil and gas, as well as critical related oil-field lifecycle services.
                </p>

                <p class="about-editorial-text">
                    Widely acknowledged for its standard-setting corporate governance practices, Transparency International has acclaimed ONGC as <span class="fw-bold">26th</span> among the single biggest publicly traded global giants. It stands resiliently as one of the most valued and consistently highest profit-making public sector enterprises driving national developmental benchmarks.
                </p>
            </div>

            <div class="col-lg-4">
                <div class="stats-sidebar-card">
                    <h3 class="sidebar-card-title">Global Rankings</h3>
                    
                    <div class="stat-metric-row">
                        <div class="stat-metric-value">20th</div>
                        <div class="stat-metric-label">Among Global Energy Majors (Platts)</div>
                    </div>

                    <div class="stat-metric-row">
                        <div class="stat-metric-value">14th</div>
                        <div class="stat-metric-label">In 'Oil & Gas Operations' Globally (Forbes)</div>
                    </div>

                    <div class="stat-metric-row">
                        <div class="stat-metric-value">220th</div>
                        <div class="stat-metric-label">Overall Position in the Forbes Global 2000</div>
                    </div>

                    <div class="stat-metric-row">
                        <div class="stat-metric-value">26th</div>
                        <div class="stat-metric-label">In Global Corporate Governance Transparency</div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>