<%@ Page Title="About Us - ONGC" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="ongc_webapp.About" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .about-page-wrapper { max-width: 1300px; margin: 15px auto 40px auto; padding: 0 40px; }
        .about-main-title { color: #1a202c; font-weight: 800; font-size: 2.6rem; letter-spacing: -0.5px; margin-bottom: 20px; position: relative; padding-bottom: 12px; }
        .about-main-title::after { content: ""; position: absolute; left: 0; bottom: 0; width: 60px; height: 4px; background-color: #7a0616; border-radius: 2px; }
        .about-asset-image { width: 100%; height: auto; max-height: 400px; object-fit: cover; border-radius: 8px; border: 1px solid #e2e8f0; margin-bottom: 35px; box-shadow: 0 4px 15px rgba(0,0,0,0.06); }
        .about-editorial-text { color: #2d3748; font-size: 1.1rem; line-height: 1.8; margin-bottom: 25px; text-align: justify; }
        .about-highlight-text { font-weight: 700; color: #7a0616; }

        /* Sidebar Container */
        .stats-sidebar-card { background-color: #fcfcfc; border: 1px solid #e2e8f0; border-radius: 16px; padding: 25px; position: sticky; top: 20px; }
        .sidebar-card-title { color: #1a202c; font-weight: 800; font-size: 1.3rem; margin-bottom: 25px; }

        /* Interactive Feature Boxes */
        .feature-box {
            background: #ffffff;
            border: 1px solid #edf2f7;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 15px;
            transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
            box-shadow: 0 2px 5px rgba(0,0,0,0.03);
            cursor: pointer;
        }

        .feature-box:hover {
            transform: translateY(-5px);
            border-color: #7a0616;
            box-shadow: 0 10px 20px rgba(122, 6, 22, 0.1);
        }

        .feature-title { font-size: 1.05rem; font-weight: 700; color: #7a0616; margin-bottom: 8px; }
        .feature-desc { font-size: 0.9rem; color: #4a5568; line-height: 1.5; }
    </style>

    <div class="about-page-wrapper">
        <div class="row g-5">
            <div class="col-lg-8 pe-lg-5">
                <h1 class="about-main-title">ONGC Document Vault</h1>
                <img src="rig.jpg" alt="ONGC Offshore Production Rig" class="about-asset-image" />
                
                <p class="about-editorial-text">
                    The <span class="about-highlight-text">ONGC Document Vault</span> is a centralized document management platform built to help engineers and operational teams quickly access the information they need. By bringing critical documents into a single, secure repository, the system eliminates fragmented storage, reduces search time, and makes information easier to manage.
                </p>

                <p class="about-editorial-text">
                    Designed with <span class="about-highlight-text">performance, security, and scalability</span> in mind, the platform uses optimized indexing and structured data organization to ensure fast and reliable document retrieval. Whether it's technical reports, project documentation, compliance records, or operational data, users can locate verified information in seconds.
                </p>

                <p class="about-editorial-text">
                    More than just a storage solution, the ONGC Document Vault improves how engineers interact with data. By simplifying document discovery and ensuring information remains accessible, organized, and secure, the platform enables teams to <span class="about-highlight-text">focus on solving engineering challenges</span> rather than searching for files.
                </p>
            </div>

            <div class="col-lg-4">
                <div class="stats-sidebar-card">
                    <h3 class="sidebar-card-title">System Capabilities</h3>
                    
                    <div class="feature-box">
                        <div class="feature-title">Reliable Data Integrity</div>
                        <div class="feature-desc">Every document is securely stored and validated to ensure information remains accurate, consistent, and trustworthy throughout its lifecycle.</div>
                    </div>

                    <div class="feature-box">
                        <div class="feature-title">Scalable Architecture</div>
                        <div class="feature-desc">Built to handle growing volumes of data without compromising performance, providing fast and reliable access whenever it's needed.</div>
                    </div>

                    <div class="feature-box">
                        <div class="feature-title">Audit & Traceability</div>
                        <div class="feature-desc">Track every document activity with detailed logs, giving teams complete visibility into uploads, updates, and access history.</div>
                    </div>

                    <div class="feature-box">
                        <div class="feature-title">Centralized Management</div>
                        <div class="feature-desc">A single platform for organizing, managing, and retrieving critical documents, making information easier to find and collaborate on.</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>