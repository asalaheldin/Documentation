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

namespace Documentation.Web.Controllers
{
    public class DocumentController : Controller
    {
        private readonly Lazy<IRepository<Document>> _documentRepo;
        public DocumentController(Lazy<IRepository<Document>> documentRepo)
        {
            _documentRepo = documentRepo;
        }

        private IRepository<Document> DocumentRepo => _documentRepo.Value;

        // GET: Document
        public ActionResult Index()
        {
            return View(DocumentRepo.GetAll(navigationProperties: new System.Linq.Expressions.Expression<Func<Document, object>>[] { x => x.Type, x => x.Updator, x => x.Creator }));
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
