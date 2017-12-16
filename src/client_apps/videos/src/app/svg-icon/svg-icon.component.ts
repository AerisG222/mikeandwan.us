import { Component, Input } from '@angular/core';

import { SvgIcon } from './svg-icon.enum';


@Component({
  selector: 'app-svg-icon',
  templateUrl: './svg-icon.component.html',
  styleUrls: ['./svg-icon.component.css']
})
export class SvgIconComponent {
  private _icon: SvgIcon;
  private _class: string;

  public viewbox: string;
  public d: string;

  @Input() set icon(theIcon: SvgIcon) {
    this._icon = theIcon;
    this.update();
  }

  get icon(): SvgIcon {
    return this._icon;
  }

  @Input() set klass(theClass: string) {
    this._class = theClass;
  }

  get klass(): string {
    return this._class;
  }

  private update(): void {
    switch (this._icon) {
      case SvgIcon.Adjust:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M768 1312V224q-148 0-273 73T297 495t-73 273 73 273 198 198 273 73zm768-544q0 209-103 385.5T1153.5 1433 768 1536t-385.5-103T103 1153.5 0 768t103-385.5T382.5 103 768 0t385.5 103T1433 382.5 1536 768z';
        break;
      case SvgIcon.Android:
        this.viewbox = '0 0 1408 1666';
        // tslint:disable-next-line:max-line-length
        this.d = 'M493 357q16 0 27.5-11.5T532 318t-11.5-27.5T493 279t-27 11.5-11 27.5 11 27.5 27 11.5zm422 0q16 0 27-11.5t11-27.5-11-27.5-27-11.5-27.5 11.5T876 318t11.5 27.5T915 357zM103 541q42 0 72 30t30 72v430q0 43-29.5 73t-72.5 30-73-30-30-73V643q0-42 30-72t73-30zm1060 19v666q0 46-32 78t-77 32h-75v227q0 43-30 73t-73 30-73-30-30-73v-227H635v227q0 43-30 73t-73 30q-42 0-72-30t-30-73l-1-227h-74q-46 0-78-32t-32-78V560h918zM931 155q107 55 171 153.5t64 215.5H241q0-117 64-215.5T477 155L406 24q-7-13 5-20 13-6 20 6l72 132q95-42 201-42t201 42l72-132q7-12 20-6 12 7 5 20zm477 488v430q0 43-30 73t-73 30q-42 0-72-30t-30-73V643q0-43 30-72.5t72-29.5q43 0 73 29.5t30 72.5z';
        break;
      case SvgIcon.ArrowCircleLeft:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1280 832V704q0-26-19-45t-45-19H714l189-189q19-19 19-45t-19-45l-91-91q-18-18-45-18t-45 18L360 632l-91 91q-18 18-18 45t18 45l91 91 362 362q18 18 45 18t45-18l91-91q18-18 18-45t-18-45L714 896h502q26 0 45-19t19-45zm256-64q0 209-103 385.5T1153.5 1433 768 1536t-385.5-103T103 1153.5 0 768t103-385.5T382.5 103 768 0t385.5 103T1433 382.5 1536 768z';
        break;
      case SvgIcon.ArrowCircleRight:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1285 768q0-27-18-45l-91-91-362-362q-18-18-45-18t-45 18l-91 91q-18 18-18 45t18 45l189 189H320q-26 0-45 19t-19 45v128q0 26 19 45t45 19h502l-189 189q-19 19-19 45t19 45l91 91q18 18 45 18t45-18l362-362 91-91q18-18 18-45zm251 0q0 209-103 385.5T1153.5 1433 768 1536t-385.5-103T103 1153.5 0 768t103-385.5T382.5 103 768 0t385.5 103T1433 382.5 1536 768z';
        break;
      case SvgIcon.ArrowLeft:
        this.viewbox = '0 0 1472 1558';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1472 715v128q0 53-32.5 90.5T1355 971H651l293 294q38 36 38 90t-38 90l-75 76q-37 37-90 37-52 0-91-37L37 869Q0 832 0 779q0-52 37-91L688 38q38-38 91-38 52 0 90 38l75 74q38 38 38 91t-38 91L651 587h704q52 0 84.5 37.5T1472 715z';
        break;
      case SvgIcon.ArrowRight:
        this.viewbox = '0 0 1472 1558';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1472 779q0 54-37 91l-651 651q-39 37-91 37-51 0-90-37l-75-75q-38-38-38-91t38-91l293-293H117q-52 0-84.5-37.5T0 843V715q0-53 32.5-90.5T117 587h704L528 293q-38-36-38-90t38-90l75-75q38-38 90-38 53 0 91 38l651 651q37 35 37 90z';
        break;
      case SvgIcon.BarChart:
        this.viewbox = '0 0 2048 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M640 768v512H384V768h256zm384-512v1024H768V256h256zm1024 1152v128H0V0h128v1408h1920zm-640-896v768h-256V512h256zm384-384v1152h-256V128h256z';
        break;
      case SvgIcon.Close:
        this.viewbox = '0 0 1188 1188';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1188 956q0 40-28 68l-136 136q-28 28-68 28t-68-28L594 866l-294 294q-28 28-68 28t-68-28L28 1024Q0 996 0 956t28-68l294-294L28 300Q0 272 0 232t28-68L164 28q28-28 68-28t68 28l294 294L888 28q28-28 68-28t68 28l136 136q28 28 28 68t-28 68L866 594l294 294q28 28 28 68z';
        break;
      case SvgIcon.Comment:
        this.viewbox = '0 0 1792 1537.33349609375';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1792 640q0 174-120 321.5t-326 233-450 85.5q-70 0-145-8-198 175-460 242-49 14-114 22-17 2-30.5-9t-17.5-29v-1q-3-4-.5-12t2-10 4.5-9.5l6-9 7-8.5 8-9q7-8 31-34.5t34.5-38 31-39.5 32.5-51 27-59 26-76q-157-89-247.5-220T0 640q0-130 71-248.5T262 187 548 50.5 896 0q244 0 450 85.5t326 233T1792 640z';
        break;
      case SvgIcon.Download:
        this.viewbox = '0 0 1664 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1280 1344q0-26-19-45t-45-19-45 19-19 45 19 45 45 19 45-19 19-45zm256 0q0-26-19-45t-45-19-45 19-19 45 19 45 45 19 45-19 19-45zm128-224v320q0 40-28 68t-68 28H96q-40 0-68-28t-28-68v-320q0-40 28-68t68-28h465l135 136q58 56 136 56t136-56l136-136h464q40 0 68 28t28 68zm-325-569q17 41-14 70l-448 448q-18 19-45 19t-45-19L339 621q-31-29-14-70 17-39 59-39h256V64q0-26 19-45t45-19h256q26 0 45 19t19 45v448h256q42 0 59 39z';
        break;
      case SvgIcon.Expand:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M755 928q0 13-10 23l-332 332 144 144q19 19 19 45t-19 45-45 19H64q-26 0-45-19t-19-45v-448q0-26 19-45t45-19 45 19l144 144 332-332q10-10 23-10t23 10l114 114q10 10 10 23zm781-864v448q0 26-19 45t-45 19-45-19l-144-144-332 332q-10 10-23 10t-23-10L791 631q-10-10-10-23t10-23l332-332-144-144q-19-19-19-45t19-45 45-19h448q26 0 45 19t19 45z';
        break;
      case SvgIcon.Home:
        this.viewbox = '0 0 1613.3333740234375 1283';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1382.667 739v480q0 26-19 45t-45 19h-384V899h-256v384h-384q-26 0-45-19t-19-45V739q0-1 .5-3t.5-3l575-474 575 474q1 2 1 6zm223-69l-62 74q-8 9-21 11h-3q-13 0-21-7l-692-577-692 577q-12 8-24 7-13-2-21-11l-62-74q-8-10-7-23.5t11-21.5l719-599q32-26 76-26t76 26l244 204V35q0-14 9-23t23-9h192q14 0 23 9t9 23v408l219 182q10 8 11 21.5t-7 23.5z';
        break;
      case SvgIcon.Image:
        this.viewbox = '0 0 1920 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M640 448q0 80-56 136t-136 56-136-56-56-136 56-136 136-56 136 56 56 136zm1024 384v448H256v-192l320-320 160 160 512-512zm96-704H160q-13 0-22.5 9.5T128 160v1216q0 13 9.5 22.5t22.5 9.5h1600q13 0 22.5-9.5t9.5-22.5V160q0-13-9.5-22.5T1760 128zm160 32v1216q0 66-47 113t-113 47H160q-66 0-113-47T0 1376V160Q0 94 47 47T160 0h1600q66 0 113 47t47 113z';
        break;
      case SvgIcon.InfoCircle:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1024 1248v-160q0-14-9-23t-23-9h-96V544q0-14-9-23t-23-9H544q-14 0-23 9t-9 23v160q0 14 9 23t23 9h96v320h-96q-14 0-23 9t-9 23v160q0 14 9 23t23 9h448q14 0 23-9t9-23zM896 352V192q0-14-9-23t-23-9H672q-14 0-23 9t-9 23v160q0 14 9 23t23 9h192q14 0 23-9t9-23zm640 416q0 209-103 385.5T1153.5 1433 768 1536t-385.5-103T103 1153.5 0 768t103-385.5T382.5 103 768 0t385.5 103T1433 382.5 1536 768z';
        break;
      case SvgIcon.Keyboard:
        this.viewbox = '0 0 1920 1152';
        // tslint:disable-next-line:max-line-length
        this.d = 'M384 784v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zm128-256v96q0 16-16 16H272q-16 0-16-16v-96q0-16 16-16h224q16 0 16 16zM384 272v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zm1024 512v96q0 16-16 16H528q-16 0-16-16v-96q0-16 16-16h864q16 0 16 16zM768 528v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zM640 272v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zm384 256v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zM896 272v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zm384 256v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zm384 256v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zm-512-512v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zm256 0v96q0 16-16 16h-96q-16 0-16-16v-96q0-16 16-16h96q16 0 16 16zm256 0v352q0 16-16 16h-224q-16 0-16-16v-96q0-16 16-16h112V272q0-16 16-16h96q16 0 16 16zm128 752V128H128v896h1664zm128-896v896q0 53-37.5 90.5T1792 1152H128q-53 0-90.5-37.5T0 1024V128q0-53 37.5-90.5T128 0h1664q53 0 90.5 37.5T1920 128z';
        break;
      case SvgIcon.MapMarker:
        this.viewbox = '0 0 1024 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M768 512q0-106-75-181t-181-75-181 75-75 181 75 181 181 75 181-75 75-181zm256 0q0 109-33 179l-364 774q-16 33-47.5 52t-67.5 19-67.5-19-46.5-52L33 691Q0 621 0 512q0-212 150-362T512 0t362 150 150 362z';
        break;
      case SvgIcon.PieChart:
        this.viewbox = '0 0 1728 1664';
        // tslint:disable-next-line:max-line-length
        this.d = 'M768 890l546 546q-106 108-247.5 168T768 1664q-209 0-385.5-103T103 1281.5 0 896t103-385.5T382.5 231 768 128v762zm187 6h773q0 157-60 298.5T1500 1442zm709-128H896V0q209 0 385.5 103T1561 382.5 1664 768z';
        break;
      case SvgIcon.Play:
        this.viewbox = '0 0 1407 1557.3333740234375';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1384 809.667l-1328 738q-23 13-39.5 3t-16.5-36v-1472q0-26 16.5-36t39.5 3l1328 738q23 13 23 31t-23 31z';
        break;
      case SvgIcon.Repeat:
      case SvgIcon.RotateRight:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1536 128v448q0 26-19 45t-45 19h-448q-42 0-59-40-17-39 14-69l138-138Q969 256 768 256q-104 0-198.5 40.5T406 406 296.5 569.5 256 768t40.5 198.5T406 1130t163.5 109.5T768 1280q119 0 225-52t179-147q7-10 23-12 14 0 25 9l137 138q9 8 9.5 20.5t-7.5 22.5q-109 132-264 204.5T768 1536q-156 0-298-61t-245-164-164-245T0 768t61-298 164-245T470 61 768 0q147 0 284.5 55.5T1297 212l130-129q29-31 70-14 39 17 39 59z';
        break;
      case SvgIcon.RotateLeft:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1536 768q0 156-61 298t-164 245-245 164-298 61q-172 0-327-72.5T177 1259q-7-10-6.5-22.5t8.5-20.5l137-138q10-9 25-9 16 2 23 12 73 95 179 147t225 52q104 0 198.5-40.5T1130 1130t109.5-163.5T1280 768t-40.5-198.5T1130 406 966.5 296.5 768 256q-98 0-188 35.5T420 393l137 138q31 30 14 69-17 40-59 40H64q-26 0-45-19T0 576V128q0-42 40-59 39-17 69 14l130 129Q346 111 483.5 55.5T768 0q156 0 298 61t245 164 164 245 61 298z';
        break;
      case SvgIcon.Star:
        this.viewbox = '0 0 1664 1587';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1664 615q0 22-26 48l-363 354 86 500q1 7 1 20 0 21-10.5 35.5T1321 1587q-19 0-40-12l-449-236-449 236q-22 12-40 12-21 0-31.5-14.5T301 1537q0-6 2-20l86-500L25 663Q0 636 0 615q0-37 56-46l502-73L783 41q19-41 49-41t49 41l225 455 502 73q56 9 56 46z';
        break;
      case SvgIcon.StarO:
        this.viewbox = '0 0 1664 1587';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1137 972l306-297-422-62-189-382-189 382-422 62 306 297-73 421 378-199 377 199zm527-357q0 22-26 48l-363 354 86 500q1 7 1 20 0 50-41 50-19 0-40-12l-449-236-449 236q-22 12-40 12-21 0-31.5-14.5T301 1537q0-6 2-20l86-500L25 663Q0 636 0 615q0-37 56-46l502-73L783 41q19-41 49-41t49 41l225 455 502 73q56 9 56 46z';
        break;
      case SvgIcon.Stop:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1536 64v1408q0 26-19 45t-45 19H64q-26 0-45-19t-19-45V64q0-26 19-45T64 0h1408q26 0 45 19t19 45z';
        break;
      case SvgIcon.TimesCircle:
        this.viewbox = '0 0 1536 1536';
        // tslint:disable-next-line:max-line-length
        this.d = 'M1149 994q0-26-19-45L949 768l181-181q19-19 19-45 0-27-19-46l-90-90q-19-19-46-19-26 0-45 19L768 587 587 406q-19-19-45-19-27 0-46 19l-90 90q-19 19-19 46 0 26 19 45l181 181-181 181q-19 19-19 45 0 27 19 46l90 90q19 19 46 19 26 0 45-19l181-181 181 181q19 19 45 19 27 0 46-19l90-90q19-19 19-46zm387-226q0 209-103 385.5T1153.5 1433 768 1536t-385.5-103T103 1153.5 0 768t103-385.5T382.5 103 768 0t385.5 103T1433 382.5 1536 768z';
        break;
      case SvgIcon.Wrench:
        this.viewbox = '0 0 1641 1643';
        // tslint:disable-next-line:max-line-length
        this.d = 'M363 1344q0-26-19-45t-45-19-45 19-19 45 19 45 45 19 45-19 19-45zm644-420l-682 682q-37 37-90 37-52 0-91-37L38 1498q-38-36-38-90 0-53 38-91l681-681q39 98 114.5 173.5T1007 924zm634-435q0 39-23 106-47 134-164.5 217.5T1195 896q-185 0-316.5-131.5T747 448t131.5-316.5T1195 0q58 0 121.5 16.5T1424 63q16 11 16 28t-16 28l-293 169v224l193 107q5-3 79-48.5t135.5-81T1609 454q15 0 23.5 10t8.5 25z';
        break;
      default:
        this.viewbox = '';
        this.d = '';
    }
  }
}
