const productNames = [
    "Amoxicillin 500mg Capsules","Lisinopril 10mg Tablets","Metformin 500mg Tablets",
    "Atorvastatin 20mg Tablets","Omeprazole 20mg Capsules","Amlodipine 5mg Tablets",
    "Metoprolol 50mg Tablets","Losartan 50mg Tablets","Albuterol HFA Inhaler",
    "Gabapentin 300mg Capsules","Hydrochlorothiazide 25mg Tablets","Sertraline 50mg Tablets",
    "Simvastatin 20mg Tablets","Montelukast 10mg Tablets","Escitalopram 10mg Tablets",
    "Rosuvastatin 10mg Tablets","Levothyroxine 50mcg Tablets","Pantoprazole 40mg Tablets",
    "Furosemide 40mg Tablets","Prednisone 10mg Tablets","Tamsulosin 0.4mg Capsules",
    "Meloxicam 15mg Tablets","Clopidogrel 75mg Tablets","Carvedilol 25mg Tablets",
    "Trazodone 50mg Tablets","Duloxetine 30mg Capsules","Pravastatin 40mg Tablets",
    "Fluoxetine 20mg Capsules","Cephalexin 500mg Capsules","Azithromycin 250mg Tablets",
    "Doxycycline 100mg Capsules","Ciprofloxacin 500mg Tablets","Ibuprofen 800mg Tablets",
    "Naproxen 500mg Tablets","Cyclobenzaprine 10mg Tablets","Tramadol 50mg Tablets",
    "Alprazolam 0.5mg Tablets","Clonazepam 1mg Tablets","Lorazepam 1mg Tablets",
    "Diazepam 5mg Tablets","Oxycodone 5mg Tablets","Hydrocodone 5mg Tablets",
    "Morphine 15mg Tablets","Warfarin 5mg Tablets","Spironolactone 25mg Tablets",
    "Finasteride 5mg Tablets","Doxazosin 4mg Tablets","Venlafaxine 75mg Capsules",
    "Bupropion 150mg Tablets","Quetiapine 100mg Tablets"
];

const manufacturers = [
    "Pfizer Inc.","Johnson & Johnson","Roche Holding AG","Novartis AG","Merck & Co.",
    "AbbVie Inc.","Bristol-Myers Squibb","AstraZeneca PLC","Eli Lilly and Company",
    "Amgen Inc.","Gilead Sciences","Sanofi S.A.","GlaxoSmithKline PLC","Novo Nordisk",
    "Bayer AG","Takeda Pharmaceutical","Boehringer Ingelheim","Teva Pharmaceutical",
    "Mylan N.V.","Allergan PLC","Biogen Inc.","Regeneron Pharmaceuticals","Vertex Pharmaceuticals",
    "Alexion Pharmaceuticals","Celgene Corporation","Shire PLC","Perrigo Company",
    "Endo International","Mallinckrodt Pharmaceuticals","Bausch Health Companies",
    "Hikma Pharmaceuticals","Sun Pharmaceutical","Dr. Reddy's Laboratories","Cipla Ltd.",
    "Lupin Limited","Aurobindo Pharma","Zydus Lifesciences","Glenmark Pharmaceuticals",
    "Torrent Pharmaceuticals","Alkem Laboratories","Sandoz International","Apotex Inc.",
    "Par Pharmaceutical","Amneal Pharmaceuticals","Lannett Company","Impax Laboratories",
    "Actavis Generics","Watson Pharmaceuticals","Barr Pharmaceuticals","Ranbaxy Laboratories"
];

const descriptions = [
    "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
    "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
    "Ut enim ad minim veniam, quis nostrud exercitation ullamco.",
    "Duis aute irure dolor in reprehenderit in voluptate velit.",
    "Excepteur sint occaecat cupidatat non proident sunt in culpa.",
    "Nemo enim ipsam voluptatem quia voluptas sit aspernatur.",
    "Neque porro quisquam est qui dolorem ipsum quia dolor sit.",
    "Quis autem vel eum iure reprehenderit qui in ea voluptate.",
    "At vero eos et accusamus et iusto odio dignissimos ducimus.",
    "Nam libero tempore cum soluta nobis est eligendi optio."
];

function pick(arr) { return arr[Math.floor(Math.random() * arr.length)]; }
function randomNDC() {
    const p = (n) => String(Math.floor(Math.random() * Math.pow(10, n))).padStart(n, '0');
    return `${p(5)}-${p(4)}-${p(2)}`;
}
function randomBarcode() {
    let s = '';
    for (let i = 0; i < 12; i++) s += Math.floor(Math.random() * 10);
    return s;
}

function randomize(id, arr, fn) {
    const el = document.getElementById(id);
    el.value = arr ? pick(arr) : fn();
    el.style.color = '';
}

function setPlaceholder(id, arr, fn) {
    const el = document.getElementById(id);
    if (el.value) return; // don't overwrite user input on validation re-render
    el.value = arr ? pick(arr) : fn();
    el.style.color = '#999';
    el.addEventListener('focus', function handler() {
        if (el.style.color === 'rgb(153, 153, 153)') el.style.color = '';
        el.removeEventListener('focus', handler);
    });
}
