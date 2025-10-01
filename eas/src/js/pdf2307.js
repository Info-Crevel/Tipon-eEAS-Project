import 
  jsPDF 
from 'jspdf';


export function createPDF(trx) {

    let pdfName = 'form2307'; 
    // var doc = new jsPDF('p', 'mm', [355.6, 215.9]);
    var doc = new jsPDF('p', 'pt','legal',true);
    var fontSize = 10;
    var imgData = 'img/form/Form2307.png'
   
    doc.setFontSize(fontSize);  
    var width = doc.internal.pageSize.getWidth();
    var height = doc.internal.pageSize.getHeight();
    doc.addImage(imgData, 'PNG', 0, 0, width, height,'','FAST');
    doc.setFont("helvetica");

    
            doc.text(trx.payeeName, 25, 191);
            doc.text(trx.address1, 25, 225);


const month = String(trx.trxDate.month).padStart(2, '0');
doc.text(month, 150, 125);

// const day = String(trx.trxDate.month).padStart(2, '0');
doc.text('0 1', 180, 125);


doc.text(String(trx.trxDate.year), 203, 125);




const monthA = String(trx.trxDate.month).padStart(2, '0');
doc.text(monthA, 407, 125);

const monthB = String(trx.trxDate.month).padStart(2, '0');

// Get last day of the month
const year = trx.trxDate.year;
const monthNew = trx.trxDate.month;
const dDay = new Date(year, monthNew, 0).getDate(); 


doc.text(String(dDay), 433, 125);     // Print DDay (last day of the month)

doc.text(String(trx.trxDate.year), 460, 125);





// doc.text(trx.trxDate, 25, 225);


    //  doc.text(trx.postalCode, 550, 225);

const startX = 550;
const y = 226;
const spacing = 13; // Adjust as needed
const postal = trx.postalCode || '';

for (let i = 0; i < postal.length; i++) {
  doc.text(postal[i], startX + i * spacing, y);
}

const startTaxX = 205;
const taxy = 162;
const spacingTax = 13;
const groupGap = 13;         
const group4ExtraGap = 8;   
const tax = trx.taxIdNumber || '';

for (let i = 0; i < tax.length; i++) {
  const groupCount = Math.floor(i / 3);
  let extra = groupCount * groupGap;

  if (i >= 9) {
    extra += group4ExtraGap;
  }

  doc.text(tax[i], startTaxX + i * spacingTax + extra, taxy);
}


   doc.text(trx.aTaxName, 10, 422);
    doc.text(trx.aTaxCode, 174, 422);
const formattedATaxAmount = trx.aTaxAmount != null
  ? trx.aTaxAmount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
  : '0.00';

  doc.text(formattedATaxAmount, 570, 422);


const formattedPayeeAmount = trx.payeeAmount != null
  ? trx.payeeAmount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
  : '0.00';

  doc.text(formattedPayeeAmount, 470, 422);




doc.text(formattedATaxAmount, 570, 582);
doc.text(formattedATaxAmount, 570, 780);

  if ([1, 4, 7, 10].includes(trx.trxDate.month)) {
  doc.text(formattedPayeeAmount, 235, 422);
    }

  if ([2, 5, 8, 11].includes(trx.trxDate.month)) {
    doc.text(formattedPayeeAmount, 310, 422);   //2nd

    }

  if ([3, 6, 9, 12].includes(trx.trxDate.month)) {
  doc.text(formattedPayeeAmount, 390, 422);   //3rd

    }

    doc.save(pdfName + '.pdf');
  
}

