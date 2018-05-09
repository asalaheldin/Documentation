using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Documentation.Data.DAL.Implementation;
using Documentation.Data.DAL.Intefraces;
using Documentation.Data.Entities;
using Documentation.Web.Helper;

namespace Documentation.Web.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly Lazy<IRepository<Document>> _documentRepo;
        public DocumentController(Lazy<IRepository<Document>> documentRepo)
        {
            _documentRepo = documentRepo;
        }

        private IRepository<Document> DocumentRepo => _documentRepo.Value;

        // GET: Document
        public ActionResult Index(int? type = null, DateTime? date = null, string query = "", string orderByField = "", string orderDir = "asc", int page = 1, int pageSize = 10, bool isPartial = false)
        {
            IQueryable<Document> model = null;

            model = DocumentRepo.GetAll(navigationProperties: new System.Linq.Expressions.Expression<Func<Document, object>>[] { x => x.Type, x => x.Updator, x => x.Creator });

            //Apply Search
            if (!string.IsNullOrEmpty(query))
            {
                model = model.Where(x => x.Subject.Trim().ToLower().Contains(query.Trim().ToLower())
                || x.SerialNumber.Trim().ToLower().Contains(query.Trim().ToLower())
                || x.Remarks.Trim().ToLower().Contains(query.Trim().ToLower())
                );
            }
            //Apply Search By Date
            if (date != null)
            {
                model = model.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date.Value));
            }
            //Apply Type Search
            if (type != null)
            {
                model = model.Where(x => x.TypeId == type.Value);
            }

            //Apply Order By
            switch (orderByField)
            {
                case "type":
                    model = orderDir == "asc" ? model.OrderBy(x => x.TypeId) : model.OrderByDescending(x => x.TypeId);
                    break;
                case "remarks":
                    model = orderDir == "asc" ? model.OrderBy(x => x.Remarks) : model.OrderByDescending(x => x.Remarks);
                    break;
                case "serial":
                    model = orderDir == "asc" ? model.OrderBy(x => x.SerialNumber) : model.OrderByDescending(x => x.SerialNumber);
                    break;
                case "date":
                    model = orderDir == "asc" ? model.OrderBy(x => x.Date) : model.OrderByDescending(x => x.Date);
                    break;
                case "subject":
                    model = orderDir == "asc" ? model.OrderBy(x => x.Subject) : model.OrderByDescending(x => x.Subject);
                    break;
                default:
                    model = orderDir == "asc" ? model.OrderBy(x => x.CreatedOn) : model.OrderByDescending(x => x.CreatedOn);
                    break;
            }

            model = model.Skip((page - 1) * pageSize).Take(pageSize);

            if (isPartial)
            {
                var _partialView = WebExtensions.RenderViewToString(this.ControllerContext, "_indexPartial", model);
                return Json(new { status = "success", partialView = _partialView }, JsonRequestBehavior.AllowGet);
            }
            return View(model);
        }

        // GET: Document/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = DocumentRepo.FindBy(z => z.Id == id.Value, navigationProperties: new System.Linq.Expressions.Expression<Func<Document, object>>[] { x => x.Type, x => x.Updator, x => x.Creator }).FirstOrDefault();
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Document/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Document/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Document document)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count <= 0 || Request.Files["Doc"] == null || Request.Files["Doc"].ContentLength <= 0)
                {
                    ModelState.AddModelError("", "Please upload a document");
                    return View(document);
                }
                var file = Request.Files["Doc"] as HttpPostedFileBase;
                document.FileName = file.FileName;
                document.FileExtension = Path.GetExtension(file.FileName);
                int result = DocumentRepo.Insert(document);
                if (result > 0)
                {
                    try
                    {
                        string targetFolder = Server.MapPath("~/Documents");
                        string targetPath = Path.Combine(targetFolder, document.Id.ToString() + document.FileExtension);
                        file.SaveAs(targetPath);
                    }
                    catch (Exception ex)
                    {
                        DocumentRepo.Delete(document.Id);
                        ModelState.AddModelError("", "problem saving the document!");
                        return View(document);

                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong, kindly try again later");
                    return View(document);
                }
            }

            return View(document);
        }

        // GET: Document/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = DocumentRepo.GetById(id.Value);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Document/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Document document)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = null;
                if (string.IsNullOrEmpty(document.FileName) && (Request.Files.Count <= 0 || Request.Files["Doc"] == null || Request.Files["Doc"].ContentLength <= 0))
                {
                    ModelState.AddModelError("", "Please upload a document");
                    return View(document);
                }
                if (Request.Files.Count > 0 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
                    file = Request.Files["Doc"] as HttpPostedFileBase;
                string currentFileName = document.FileName,
                    currentFileExtension = document.FileExtension;

                document.FileName = file?.FileName ?? document.FileName;
                document.FileExtension = file?.FileName != null ? Path.GetExtension(file.FileName) : document.FileExtension;
                int result = DocumentRepo.Update(document, document.Id);
                if (result > 0)
                {
                    try
                    {
                        string targetFolder = Server.MapPath("~/Documents");
                        string targetPath = Path.Combine(targetFolder, document.Id.ToString() + document.FileExtension);
                        file.SaveAs(targetPath);
                    }
                    catch (Exception ex)
                    {
                        document.FileName = currentFileName;
                        document.FileExtension = currentFileExtension;
                        DocumentRepo.Update(document, document.Id);
                        ModelState.AddModelError("", "problem saving the document!");
                        return View(document);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong, kindly try again later");
                    return View(document);
                }
            }
            return View(document);
        }

        // GET: Document/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = DocumentRepo.GetById(id.Value);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            int result = DocumentRepo.Delete(id);
            if (result > 0)
                return RedirectToAction("Index");
            else
            {
                ModelState.AddModelError("", "Something went wrong, kindly try again later");
                return View();
            }
        }
        public FileResult Download(Guid id, string fileName)
        {
            string path = Path.Combine(Server.MapPath("~/Documents"), id.ToString() + Path.GetExtension(fileName));
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
