using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Maw.TagHelpers;

// icons pulled from source files at https://leungwensen.github.io/svg-icon/#awesome
[HtmlTargetElementAttribute("svg-icon")]
public class SvgIconTagHelper
    : TagHelper
{
    [HtmlAttributeNameAttribute("icon")]
    public SvgIcon Icon { get; set; }

    [HtmlAttributeNameAttribute("class")]
    public string? Klass { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (output == null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        output.TagName = "span";
        output.Attributes.SetAttribute("class", $"svg-icon svg-baseline {Klass}");

        string v;
        string d;

        switch (Icon)
        {
            case SvgIcon.Android:
                v = "0 0 1408 1666";
                d = "M493 357q16 0 27.5-11.5T532 318t-11.5-27.5T493 279t-27 11.5-11 27.5 11 27.5 27 11.5zm422 0q16 0 27-11.5t11-27.5-11-27.5-27-11.5-27.5 11.5T876 318t11.5 27.5T915 357zM103 541q42 0 72 30t30 72v430q0 43-29.5 73t-72.5 30-73-30-30-73V643q0-42 30-72t73-30zm1060 19v666q0 46-32 78t-77 32h-75v227q0 43-30 73t-73 30-73-30-30-73v-227H635v227q0 43-30 73t-73 30q-42 0-72-30t-30-73l-1-227h-74q-46 0-78-32t-32-78V560h918zM931 155q107 55 171 153.5t64 215.5H241q0-117 64-215.5T477 155L406 24q-7-13 5-20 13-6 20 6l72 132q95-42 201-42t201 42l72-132q7-12 20-6 12 7 5 20zm477 488v430q0 43-30 73t-73 30q-42 0-72-30t-30-73V643q0-43 30-72.5t72-29.5q43 0 73 29.5t30 72.5z";
                break;
            case SvgIcon.At:
                v = "0 0 1536 1536";
                d = "M972 647q0-108-53.5-169T771 417q-63 0-124 30.5T537 532t-79.5 137T427 849q0 112 53.5 173t150.5 61q96 0 176-66.5t122.5-166T972 647zm564 121q0 111-37 197t-98.5 135-131.5 74.5-145 27.5q-6 0-15.5.5t-16.5.5q-95 0-142-53-28-33-33-83-52 66-131.5 110T612 1221q-161 0-249.5-95.5T274 856q0-157 66-290t179-210.5T765 278q87 0 155 35.5t106 99.5l2-19 11-56q1-6 5.5-12t9.5-6h118q5 0 13 11 5 5 3 16l-120 614q-5 24-5 48 0 39 12.5 52t44.5 13q28-1 57-5.5t73-24 77-50 57-89.5 24-137q0-292-174-466T768 128q-130 0-248.5 51t-204 136.5-136.5 204T128 768t51 248.5 136.5 204 204 136.5 248.5 51q228 0 405-144 11-9 24-8t21 12l41 49q8 12 7 24-2 13-12 22-102 83-227.5 128T768 1536q-156 0-298-61t-245-164-164-245T0 768t61-298 164-245T470 61 768 0q344 0 556 212t212 556z";
                break;
            case SvgIcon.Bars:
                v = "0 0 1536 1280";
                d = "M1536 1088v128q0 26-19 45t-45 19H64q-26 0-45-19t-19-45v-128q0-26 19-45t45-19h1408q26 0 45 19t19 45zm0-512v128q0 26-19 45t-45 19H64q-26 0-45-19T0 704V576q0-26 19-45t45-19h1408q26 0 45 19t19 45zm0-512v128q0 26-19 45t-45 19H64q-26 0-45-19T0 192V64q0-26 19-45T64 0h1408q26 0 45 19t19 45z";
                break;
            case SvgIcon.Camera:
                v = "0 0 1920 1664";
                d = "M960 672q119 0 203.5 84.5T1248 960t-84.5 203.5T960 1248t-203.5-84.5T672 960t84.5-203.5T960 672zm704-416q106 0 181 75t75 181v896q0 106-75 181t-181 75H256q-106 0-181-75T0 1408V512q0-106 75-181t181-75h224l51-136q19-49 69.5-84.5T704 0h512q53 0 103.5 35.5T1389 120l51 136h224zM960 1408q185 0 316.5-131.5T1408 960t-131.5-316.5T960 512 643.5 643.5 512 960t131.5 316.5T960 1408z";
                break;
            case SvgIcon.CaretDown:
                v = "0 0 1024 576";
                d = "M1024 64q0 26-19 45L557 557q-19 19-45 19t-45-19L19 109Q0 90 0 64t19-45T64 0h896q26 0 45 19t19 45z";
                break;
            case SvgIcon.Check:
                v = "0 0 1550 1188";
                d = "M1550 232q0 40-28 68l-724 724-136 136q-28 28-68 28t-68-28l-136-136L28 662Q0 634 0 594t28-68l136-136q28-28 68-28t68 28l294 295 656-657q28-28 68-28t68 28l136 136q28 28 28 68z";
                break;
            case SvgIcon.Cog:
                v = "0 0 1536 1536";
                d = "M1024 768q0-106-75-181t-181-75-181 75-75 181 75 181 181 75 181-75 75-181zm512-109v222q0 12-8 23t-20 13l-185 28q-19 54-39 91 35 50 107 138 10 12 10 25t-9 23q-27 37-99 108t-94 71q-12 0-26-9l-138-108q-44 23-91 38-16 136-29 186-7 28-36 28H657q-14 0-24.5-8.5T621 1506l-28-184q-49-16-90-37l-141 107q-10 9-25 9-14 0-25-11-126-114-165-168-7-10-7-23 0-12 8-23 15-21 51-66.5t54-70.5q-27-50-41-99L29 913q-13-2-21-12.5T0 877V655q0-12 8-23t19-13l186-28q14-46 39-92-40-57-107-138-10-12-10-24 0-10 9-23 26-36 98.5-107.5T337 135q13 0 26 10l138 107q44-23 91-38 16-136 29-186 7-28 36-28h222q14 0 24.5 8.5T915 30l28 184q49 16 90 37l142-107q9-9 24-9 13 0 25 10 129 119 165 170 7 8 7 22 0 12-8 23-15 21-51 66.5t-54 70.5q26 50 41 98l183 28q13 2 21 12.5t8 23.5z";
                break;
            case SvgIcon.Download:
                v = "0 0 1664 1536";
                d = "M1280 1344q0-26-19-45t-45-19-45 19-19 45 19 45 45 19 45-19 19-45zm256 0q0-26-19-45t-45-19-45 19-19 45 19 45 45 19 45-19 19-45zm128-224v320q0 40-28 68t-68 28H96q-40 0-68-28t-28-68v-320q0-40 28-68t68-28h465l135 136q58 56 136 56t136-56l136-136h464q40 0 68 28t28 68zm-325-569q17 41-14 70l-448 448q-18 19-45 19t-45-19L339 621q-31-29-14-70 17-39 59-39h256V64q0-26 19-45t45-19h256q26 0 45 19t19 45v448h256q42 0 59 39z";
                break;
            case SvgIcon.Envelope:
                v = "0 0 1792 1408";
                d = "M1792 454v794q0 66-47 113t-113 47H160q-66 0-113-47T0 1248V454q44 49 101 87 362 246 497 345 57 42 92.5 65.5t94.5 48 110 24.5h2q51 0 110-24.5t94.5-48T1194 886q170-123 498-345 57-39 100-87zm0-294q0 79-49 151t-122 123q-376 261-468 325-10 7-42.5 30.5t-54 38-52 32.5-57.5 27-50 9h-2q-23 0-50-9t-57.5-27-52-32.5-54-38T639 759q-91-64-262-182.5T172 434q-62-42-117-115.5T0 182q0-78 41.5-130T160 0h1472q65 0 112.5 47t47.5 113z";
                break;
            case SvgIcon.Facebook:
                v = "0 0 1536 1536";
                d = "M1451 0q35 0 60 25t25 60v1366q0 35-25 60t-60 25h-391V941h199l30-232h-229V561q0-56 23.5-84t91.5-28l122-1V241q-63-9-178-9-136 0-217.5 80T820 538v171H620v232h200v595H85q-35 0-60-25t-25-60V85q0-35 25-60T85 0h1366z";
                break;
            case SvgIcon.Flag:
                v = "0 0 1728 1536";
                d = "M256 128q0 72-64 110v1266q0 13-9.5 22.5T160 1536H96q-13 0-22.5-9.5T64 1504V238Q0 200 0 128q0-53 37.5-90.5T128 0t90.5 37.5T256 128zm1472 64v763q0 25-12.5 38.5T1676 1021q-215 116-369 116-61 0-123.5-22t-108.5-48-115.5-48T817 997q-192 0-464 146-17 9-33 9-26 0-45-19t-19-45V346q0-32 31-55 21-14 79-43 236-120 421-120 107 0 200 29t219 88q38 19 88 19 54 0 117.5-21t110-47 88-47 54.5-21q26 0 45 19t19 45z";
                break;
            case SvgIcon.Github:
                v = "0 0 1536 1500.3333740234375";
                d = "M768 0q209 0 385.5 103T1433 382.5 1536 768q0 251-146.5 451.5T1011 1497q-27 5-40-7t-13-30q0-3 .5-76.5t.5-134.5q0-97-52-142 57-6 102.5-18t94-39 81-66.5 53-105T1258 728q0-119-79-206 37-91-8-204-28-9-81 11t-92 44l-38 24q-93-26-192-26t-192 26q-16-11-42.5-27T450 331.5 365 318q-45 113-8 204-79 87-79 206 0 85 20.5 150T351 983t80.5 67 94 39 102.5 18q-39 36-49 103-21 10-45 15t-57 5-65.5-21.5T356 1146q-19-32-48.5-52t-49.5-24l-20-3q-21 0-29 4.5t-5 11.5 9 14 13 12l7 5q22 10 43.5 38t31.5 51l10 23q13 38 44 61.5t67 30 69.5 7 55.5-3.5l23-4q0 38 .5 88.5t.5 54.5q0 18-13 30t-40 7q-232-77-378.5-277.5T0 768q0-209 103-385.5T382.5 103 768 0zM291 1103q3-7-7-12-10-3-13 2-3 7 7 12 9 6 13-2zm31 34q7-5-2-16-10-9-16-3-7 5 2 16 10 10 16 3zm30 45q9-7 0-19-8-13-17-6-9 5 0 18t17 7zm42 42q8-8-4-19-12-12-20-3-9 8 4 19 12 12 20 3zm57 25q3-11-13-16-15-4-19 7t13 15q15 6 19-6zm63 5q0-13-17-11-16 0-16 11 0 13 17 11 16 0 16-11zm58-10q-2-11-18-9-16 3-14 15t18 8 14-14z";
                break;
            case SvgIcon.GooglePlus:
                v = "0 0 2304 1466";
                d = "M1437 750q0 208-87 370.5t-248 254-369 91.5q-149 0-285-58t-234-156-156-234T0 733t58-285 156-234T448 58 733 0q286 0 491 192l-199 191Q908 270 733 270q-123 0-227.5 62T340 500.5 279 733t61 232.5T505.5 1134t227.5 62q83 0 152.5-23t114.5-57.5 78.5-78.5 49-83 21.5-74H733V628h692q12 63 12 122zm867-122v210h-209v209h-210V838h-209V628h209V419h210v209h209z";
                break;
            case SvgIcon.GooglePlusSquare:
                v = "0 0 1536 1536";
                d = "M917 777q0-26-6-64H549v132h217q-3 24-16.5 50T712 948t-66.5 44.5T549 1010q-99 0-169-71t-70-171 70-171 169-71q92 0 153 59l104-101Q698 384 549 384q-160 0-272 112.5T165 768t112 271.5T549 1152q165 0 266.5-105T917 777zm345 46h109V713h-109V603h-110v110h-110v110h110v110h110V823zm274-535v960q0 119-84.5 203.5T1248 1536H288q-119 0-203.5-84.5T0 1248V288Q0 169 84.5 84.5T288 0h960q119 0 203.5 84.5T1536 288z";
                break;
            case SvgIcon.Group:
                v = "0 0 1920 1792";
                d = "M593 896q-162 5-265 128H194q-82 0-138-40.5T0 865q0-353 124-353 6 0 43.5 21t97.5 42.5T384 597q67 0 133-23-5 37-5 66 0 139 81 256zm1071 637q0 120-73 189.5t-194 69.5H523q-121 0-194-69.5T256 1533q0-53 3.5-103.5t14-109T300 1212t43-97.5 62-81 85.5-53.5T602 960q10 0 43 21.5t73 48 107 48 135 21.5 135-21.5 107-48 73-48 43-21.5q61 0 111.5 20t85.5 53.5 62 81 43 97.5 26.5 108.5 14 109 3.5 103.5zM640 256q0 106-75 181t-181 75-181-75-75-181 75-181T384 0t181 75 75 181zm704 384q0 159-112.5 271.5T960 1024 688.5 911.5 576 640t112.5-271.5T960 256t271.5 112.5T1344 640zm576 225q0 78-56 118.5t-138 40.5h-134q-103-123-265-128 81-117 81-256 0-29-5-66 66 23 133 23 59 0 119-21.5t97.5-42.5 43.5-21q124 0 124 353zm-128-609q0 106-75 181t-181 75-181-75-75-181 75-181 181-75 181 75 75 181z";
                break;
            case SvgIcon.LinkedIn:
                v = "0 0 1536 1536";
                d = "M237 1286h231V592H237v694zm246-908q-1-52-36-86t-93-34-94.5 34-36.5 86q0 51 35.5 85.5T351 498h1q59 0 95-34.5t36-85.5zm585 908h231V888q0-154-73-233t-193-79q-136 0-209 117h2V592H595q3 66 0 694h231V898q0-38 7-56 15-35 45-59.5t74-24.5q116 0 116 157v371zm468-998v960q0 119-84.5 203.5T1248 1536H288q-119 0-203.5-84.5T0 1248V288Q0 169 84.5 84.5T288 0h960q119 0 203.5 84.5T1536 288z";
                break;
            case SvgIcon.Lock:
                v = "0 0 1152 1408";
                d = "M320 640h512V448q0-106-75-181t-181-75-181 75-75 181v192zm832 96v576q0 40-28 68t-68 28H96q-40 0-68-28t-28-68V736q0-40 28-68t68-28h32V448q0-184 132-316T576 0t316 132 132 316v192h32q40 0 68 28t28 68z";
                break;
            case SvgIcon.Person:
                v = "0 0 1408 1536";
                d = "M1408 1277q0 120-73 189.5t-194 69.5H267q-121 0-194-69.5T0 1277q0-53 3.5-103.5t14-109T44 956t43-97.5 62-81 85.5-53.5T346 704q9 0 42 21.5t74.5 48 108 48T704 843t133.5-21.5 108-48 74.5-48 42-21.5q61 0 111.5 20t85.5 53.5 62 81 43 97.5 26.5 108.5 14 109 3.5 103.5zm-320-893q0 159-112.5 271.5T704 768 432.5 655.5 320 384t112.5-271.5T704 0t271.5 112.5T1088 384z";
                break;
            case SvgIcon.Steam:
                v = "0 0 1792 1536";
                d = "M1582 454q0 101-71.5 172.5T1338 698t-172.5-71.5T1094 454t71.5-172.5T1338 210t172.5 71.5T1582 454zm-770 742q0-104-73-177t-177-73q-27 0-54 6l104 42q77 31 109.5 106.5T723 1252q-31 77-107 109t-152 1q-21-8-62-24.5t-61-24.5q32 60 91 96.5t130 36.5q104 0 177-73t73-177zm830-741q0-126-89.5-215.5T1337 150q-127 0-216.5 89.5T1031 455q0 127 89.5 216t216.5 89q126 0 215.5-89t89.5-216zm150 0q0 189-133.5 322T1337 910l-437 319q-12 129-109 218t-229 89q-121 0-214-76t-118-192L0 1176V747l389 157q79-48 173-48 13 0 35 2l284-407q2-187 135.5-319T1337 0q188 0 321.5 133.5T1792 455z";
                break;
            case SvgIcon.Trash:
                v = "0 0 1408 1536";
                d = "M512 1248V544q0-14-9-23t-23-9h-64q-14 0-23 9t-9 23v704q0 14 9 23t23 9h64q14 0 23-9t9-23zm256 0V544q0-14-9-23t-23-9h-64q-14 0-23 9t-9 23v704q0 14 9 23t23 9h64q14 0 23-9t9-23zm256 0V544q0-14-9-23t-23-9h-64q-14 0-23 9t-9 23v704q0 14 9 23t23 9h64q14 0 23-9t9-23zM480 256h448l-48-117q-7-9-17-11H546q-10 2-17 11zm928 32v64q0 14-9 23t-23 9h-96v948q0 83-47 143.5t-113 60.5H288q-66 0-113-58.5T128 1336V384H32q-14 0-23-9t-9-23v-64q0-14 9-23t23-9h309l70-167q15-37 54-63t79-26h320q40 0 79 26t54 63l70 167h309q14 0 23 9t9 23z";
                break;
            case SvgIcon.Trello:
                v = "0 0 1536 1536";
                d = "M704 1216V192q0-14-9-23t-23-9H192q-14 0-23 9t-9 23v1024q0 14 9 23t23 9h480q14 0 23-9t9-23zm672-384V192q0-14-9-23t-23-9H864q-14 0-23 9t-9 23v640q0 14 9 23t23 9h480q14 0 23-9t9-23zm160-768v1408q0 26-19 45t-45 19H64q-26 0-45-19t-19-45V64q0-26 19-45T64 0h1408q26 0 45 19t19 45z";
                break;
            case SvgIcon.Twitter:
                v = "0 0 1576 1280";
                d = "M1576 152q-67 98-162 167 1 14 1 42 0 130-38 259.5T1261.5 869 1077 1079.5t-258 146-323 54.5q-271 0-496-145 35 4 78 4 225 0 401-138-105-2-188-64.5T177 777q33 5 61 5 43 0 85-11-112-23-185.5-111.5T64 454v-4q68 38 146 41-66-44-105-115T66 222q0-88 44-163 121 149 294.5 238.5T776 397q-8-38-8-74 0-134 94.5-228.5T1091 0q140 0 236 102 109-21 205-78-37 115-142 178 93-10 186-50z";
                break;
            case SvgIcon.UnlockAlt:
                v = "0 0 1152 1536";
                d = "M1056 768q40 0 68 28t28 68v576q0 40-28 68t-68 28H96q-40 0-68-28t-28-68V864q0-40 28-68t68-28h32V448q0-185 131.5-316.5T576 0t316.5 131.5T1024 448q0 26-19 45t-45 19h-64q-26 0-45-19t-19-45q0-106-75-181t-181-75-181 75-75 181v320h736z";
                break;
            case SvgIcon.Upload:
                v = "0 0 1664 1600";
                d = "M1280 1408q0-26-19-45t-45-19-45 19-19 45 19 45 45 19 45-19 19-45zm256 0q0-26-19-45t-45-19-45 19-19 45 19 45 45 19 45-19 19-45zm128-224v320q0 40-28 68t-68 28H96q-40 0-68-28t-28-68v-320q0-40 28-68t68-28h427q21 56 70.5 92t110.5 36h256q61 0 110.5-36t70.5-92h427q40 0 68 28t28 68zm-325-648q-17 40-59 40h-256v448q0 26-19 45t-45 19H704q-26 0-45-19t-19-45V576H384q-42 0-59-40-17-39 14-69L787 19q18-19 45-19t45 19l448 448q31 30 14 69z";
                break;
            case SvgIcon.VideoCamera:
                v = "0 0 1792 1280";
                d = "M1792 96v1088q0 42-39 59-13 5-25 5-27 0-45-19l-403-403v166q0 119-84.5 203.5T992 1280H288q-119 0-203.5-84.5T0 992V288Q0 169 84.5 84.5T288 0h704q119 0 203.5 84.5T1280 288v165l403-402q18-19 45-19 12 0 25 5 39 17 39 59z";
                break;
            case SvgIcon.Windows:
                v = "0 0 1664 1664";
                d = "M682 878v651L0 1435V878h682zm0-743v659H0V229zm982 743v786l-907-125V878h907zm0-878v794H757V125z";
                break;
            case SvgIcon.Wrench:
                v = "0 0 1641 1643";
                d = "M363 1344q0-26-19-45t-45-19-45 19-19 45 19 45 45 19 45-19 19-45zm644-420l-682 682q-37 37-90 37-52 0-91-37L38 1498q-38-36-38-90 0-53 38-91l681-681q39 98 114.5 173.5T1007 924zm634-435q0 39-23 106-47 134-164.5 217.5T1195 896q-185 0-316.5-131.5T747 448t131.5-316.5T1195 0q58 0 121.5 16.5T1424 63q16 11 16 28t-16 28l-293 169v224l193 107q5-3 79-48.5t135.5-81T1609 454q15 0 23.5 10t8.5 25z";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        output.Content.AppendHtml($"<svg viewBox=\"{v}\" width=\"24\" height=\"24\"><path d=\"{d}\" /></svg>");
    }
}
