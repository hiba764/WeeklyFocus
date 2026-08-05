// // =============================
// // Helpers
// // =============================
// const $ = (s, el = document) => el.querySelector(s);
// const $$ = (s, el = document) => [...el.querySelectorAll(s)];

// function clamp(n, a, b) { return Math.max(a, Math.min(b, n)); }

// function showToast(msg) {
//     const t = $("#toast");
//     $("#toastMsg").textContent = msg;
//     t.style.display = "block";
//     clearTimeout(showToast._t);
//     showToast._t = setTimeout(() => t.style.display = "none", 2400);
// }

// function setTheme(theme) {
//     document.documentElement.setAttribute("data-theme", theme);
//     localStorage.setItem("wf_theme", theme);
// }
// const themeSaved = localStorage.getItem("wf_theme");
// if (themeSaved) setTheme(themeSaved);
// else setTheme("dark");

// $("#themeToggle").addEventListener("click", () => {
//     const cur = document.documentElement.getAttribute("data-theme") || "dark";
//     setTheme(cur === "dark" ? "light" : "dark");
// });

// // =============================
// // Mock API data (replace later)
// // =============================
// const mock = {
//     dashboard: {
//         week: {
//             progressPct: 65,
//             targetPct: 80,
//             xp: 160,
//             tasksTotal: 12,
//             statusText: "أنت في المسار الصحيح!"
//         },
//         todayTasks: [
//             {
//                 id: 101, title: "إنهاء API المشروع",
//                 priority: "high", difficulty: "hard",
//                 expectedMinutes: 45, dueDate: new Date().toISOString().slice(0, 10),
//                 status: "pending"
//             },
//             {
//                 id: 102, title: "مراجعة HTML",
//                 priority: "medium", difficulty: "medium",
//                 expectedMinutes: 30, dueDate: new Date().toISOString().slice(0, 10),
//                 status: "done",
//                 actualMinutes: 28,
//                 description: "تثبيت بنية الصفحة والتأكد من RTL.",
//                 imageUrl: null
//             },
//             {
//                 id: 103, title: "قراءة 10 صفحات",
//                 priority: "low", difficulty: "easy",
//                 expectedMinutes: 25, dueDate: new Date().toISOString().slice(0, 10),
//                 status: "pending"
//             }
//         ],
//     },
//     mirror: {
//         totalTasks: 12,
//         doneTasks: 9,
//         doneRatePct: 75,
//         xp: 240,
//         mostFailReason: "لم يكن لدي وقت",
//         hardestDoneTitle: "🚀 إنهاء مشروع البرمجة",
//         motivation: "👏 ممتاز! حاول الأسبوع القادم ألا تضيف أكثر من 8 مهام في اليوم."
//     },
// };

// // =============================
// // State
// // =============================
// let appState = {
//     tasks: [],
//     week: null,
//     mirror: null
// };

// // =============================
// // UI: loading/error management
// // =============================
// function setDashLoading(isLoading) {
//     $("#dashLoading").classList.toggle("hidden", !isLoading);
//     $("#dashError").classList.add("hidden");
//     $("#dashContent").classList.toggle("hidden", isLoading);
// }
// function setDashError(err) {
//     $("#dashLoading").classList.add("hidden");
//     $("#dashContent").classList.add("hidden");
//     $("#dashError").classList.remove("hidden");
//     $("#dashErrorMsg").textContent = err || "حدث خطأ أثناء الاتصال بالخادم.";
// }

// function setMirrorLoading(isLoading) {
//     $("#mirrorLoading").classList.toggle("hidden", !isLoading);
//     $("#mirrorError").classList.add("hidden");
//     $("#mirrorContent").classList.toggle("hidden", isLoading);
// }
// function setMirrorError(err) {
//     $("#mirrorLoading").classList.add("hidden");
//     $("#mirrorContent").classList.add("hidden");
//     $("#mirrorError").classList.remove("hidden");
//     $("#mirrorErrorMsg").textContent = err || "حدث خطأ أثناء الاتصال بالخادم.";
// }

// function statusLabel(status) {
//     if (status === "pending") return { text: "في الانتظار", cls: "s-pending" };
//     if (status === "done") return { text: "منجزة", cls: "s-done" };
//     if (status === "fail") return { text: "فاشلة", cls: "s-fail" };
//     return { text: "—", cls: "s-pending" };
// }

// function priorityToBadge(p) {
//     if (p === "low") return "🟢 منخفضة";
//     if (p === "medium") return "🟡 متوسطة";
//     if (p === "high") return "🔴 عالية";
//     return p;
// }
// function difficultyToBadge(d) {
//     if (d === "easy") return "🟦 سهلة";
//     if (d === "medium") return "🟠 متوسطة";
//     if (d === "hard") return "🟥 صعبة";
//     return d;
// }

// function renderDashboard() {
//     const { week, todayTasks } = appState.dashboard;
//     $("#topWeekProgressText").textContent = `${week.progressPct}%`;
//     $("#weekProgressPct").textContent = `${week.progressPct}%`;
//     $("#weekTargetText").textContent = `الهدف: ${week.targetPct}%`;
//     $("#weekTargetInline").textContent = `${week.targetPct}%`;
//     $("#weekXpInline").textContent = `${week.xp} XP`;

//     // status
//     $("#weekStatusText").textContent = week.statusText || "—";
//     $("#weekTasksCount").textContent = `${week.tasksTotal ?? 0} مهمة`;

//     $("#todayTasksCount").textContent = todayTasks?.length || 0;

//     // progress ring
//     const pct = clamp(week.progressPct ?? 0, 0, 100);
//     const r = 48;
//     const c = 2 * Math.PI * r;
//     const offset = c - (pct / 100) * c;
//     $("#progressCircle").setAttribute("stroke-dashoffset", offset);
//     $("#progressBar").style.width = pct + "%";

//     // Empty state
//     $("#todayEmptyState").classList.toggle("hidden", !!(todayTasks && todayTasks.length));
//     const list = $("#todayTasksList");
//     list.innerHTML = "";

//     if (!todayTasks || todayTasks.length === 0) return;

//     todayTasks.forEach(task => {
//         const s = statusLabel(task.status);
//         const card = document.createElement("div");
//         card.className = "task-card";
//         card.setAttribute("role", "button");
//         card.tabIndex = 0;

//         card.innerHTML = `
//           <div class="task-left">
//             <div class="status-dot ${s.cls}" aria-hidden="true"></div>
//             <div class="task-info">
//               <p class="task-title">${task.title}</p>
//               <div class="task-sub">
//                 <span class="chip">⭐ <strong>${priorityToBadge(task.priority)}</strong></span>
//                 <span class="chip">💪 <strong>${difficultyToBadge(task.difficulty)}</strong></span>
//                 <span class="chip">⏱️ <strong>${task.expectedMinutes} دقيقة</strong></span>
//                 <span class="chip">📅 <strong>${task.dueDate || "—"}</strong></span>
//               </div>
//             </div>
//           </div>
//           <div class="task-right">
//             <div class="task-actions" data-task-actions>
//               ${task.status === "pending" ? `
//                 <button class="btn small primary" data-action="done" data-id="${task.id}">تم</button>
//                 <button class="btn small" data-action="edit" data-id="${task.id}">تعديل</button>
//                 <button class="btn small danger" data-action="fail" data-id="${task.id}">فشلت</button>
//               ` : task.status === "done" ? `
//                 <div class="status-text done">✅ تمت المهمة</div>
//               ` : `
//                 <div class="status-text fail">❌ فشلت</div>
//               `}
//             </div>
//           </div>
//         `;

//         // click to details (except action buttons)
//         card.addEventListener("click", (e) => {
//             const btn = e.target.closest("button");
//             if (btn) return;
//             openDetails(task.id);
//         });
//         card.addEventListener("keydown", (e) => {
//             if (e.key === "Enter") openDetails(task.id);
//         });

//         // action delegation
//         card.querySelectorAll("[data-action]").forEach(b => {
//             b.addEventListener("click", (e) => {
//                 e.stopPropagation();
//                 const id = Number(b.dataset.id);
//                 const action = b.dataset.action;
//                 if (action === "done") openComplete(id);
//                 if (action === "fail") openFail(id);
//                 if (action === "edit") showToast("ميزة تعديل المهمة: سيتم ربطها لاحقاً (UI جاهزة).");
//             });
//         });

//         list.appendChild(card);
//     });
// }

// function renderMirror() {
//     const mirror = appState.mirror;
//     const has = mirror && (mirror.totalTasks ?? 0) > 0;

//     $("#mirrorContent").classList.toggle("hidden", false);
//     $("#mirrorEmptyState").classList.toggle("hidden", has);

//     if (!has) {
//         return;
//     }

//     $("#kpiTotalTasks").textContent = mirror.totalTasks ?? 0;
//     $("#kpiDoneTasks").textContent = mirror.doneTasks ?? 0;
//     $("#kpiDoneRate").textContent = `${mirror.doneRatePct ?? 0}%`;
//     $("#kpiDoneRateSub").textContent = (mirror.doneRatePct ?? 0) >= 80
//         ? "مستوى ممتاز — حافظ على الزخم."
//         : (mirror.doneRatePct ?? 0) >= 60
//             ? "جيد — ركّز على تقليل التسويف."
//             : "قدّم خطوات صغيرة اليوم — التحسن يبدأ من الآن.";
//     $("#kpiXp").textContent = `${mirror.xp ?? 0} XP`;
//     $("#kpiMostFailReason").textContent = mirror.mostFailReason || "—";
//     $("#kpiHardestDone").textContent = mirror.hardestDoneTitle || "—";
//     $("#kpiMotivation").textContent = mirror.motivation || "—";
// }

// // =============================
// // Fetch (mock now, replace with API)
// // =============================
// async function loadAll() {
//     try {
//         setDashLoading(true);
//         setMirrorLoading(true);

//         // simulate latency
//         await new Promise(r => setTimeout(r, 550));

//         const dashRes = mock.dashboard;
//         const mirrorRes = mock.mirror;

//         appState.dashboard = dashRes;
//         appState.mirror = mirrorRes;

//         $("#dashContent").classList.remove("hidden");
//         renderDashboard();

//         $("#mirrorContent").classList.remove("hidden");
//         $("#mirrorContent").classList.toggle("hidden", false);

//         renderMirror();

//         $("#mirrorLoading").classList.add("hidden");
//         $("#dashLoading").classList.add("hidden");

//     } catch (err) {
//         setDashError(String(err?.message || err));
//         setMirrorError(String(err?.message || err));
//     }
// }

// // initial load
// loadAll();

// // Retry
// $("#btnRetryDash").addEventListener("click", loadAll);
// $("#btnRetryMirror").addEventListener("click", loadAll);

// // =============================
// // Modals open/close
// // =============================
// function bindModal(modalEl, closeBtnId) {
//     const closeBtn = $("#" + closeBtnId);
//     closeBtn.addEventListener("click", () => modalEl.classList.remove("show"));
//     modalEl.addEventListener("click", (e) => {
//         if (e.target === modalEl) modalEl.classList.remove("show");
//     });
//     document.addEventListener("keydown", (e) => {
//         if (e.key === "Escape") modalEl.classList.remove("show");
//     });
// }

// const modalAdd = $("#modalAdd");
// const modalComplete = $("#modalComplete");
// const modalFail = $("#modalFail");
// const modalDetails = $("#modalDetails");

// bindModal(modalAdd, "closeAdd");
// bindModal(modalComplete, "closeComplete");
// bindModal(modalFail, "closeFail");
// bindModal(modalDetails, "closeDetails");

// $("#cancelAdd").addEventListener("click", () => modalAdd.classList.remove("show"));
// $("#cancelComplete").addEventListener("click", () => modalComplete.classList.remove("show"));
// $("#cancelFail").addEventListener("click", () => modalFail.classList.remove("show"));

// $("#btnEmptyAdd").addEventListener("click", () => modalAdd.classList.add("show"));
// $("#btnEmptyStartWeek").addEventListener("click", () => { modalAdd.classList.add("show"); });

// // open add
// $("#btnOpenAddTask").addEventListener("click", () => modalAdd.classList.add("show"));

// // radio tiles active state
// function syncTiles(tilesRoot) {
//     const tiles = $$("#priorityTiles .tile, #difficultyTiles .tile, #failReasonsTiles .tile");
//     tiles.forEach(tile => {
//         const input = tile.querySelector("input");
//         if (!input) return;
//         tile.classList.toggle("active", input.checked);
//         tile.addEventListener("click", () => {
//             const group = input.name;
//             document.querySelectorAll(`input[name="${group}"]`).forEach(r => r.checked = false);
//             input.checked = true;
//             syncTiles(tilesRoot);
//         }, { once: false });
//     });
// }
// // Initial sync
// syncTiles();

// // =============================
// // Add Task save
// // =============================
// $("#saveAddTask").addEventListener("click", async () => {
//     const form = $("#addForm");
//     const data = {
//         title: $("#addTitle").value.trim(),
//         description: $("#addDesc").value.trim(),
//         priority: form.querySelector('input[name="priority"]:checked')?.value,
//         difficulty: form.querySelector('input[name="difficulty"]:checked')?.value,
//         expectedMinutes: Number($("#addExpected").value),
//         dueDate: $("#addDate").value || null,
//     };

//     if (!data.title) {
//         showToast("يرجى إدخال اسم المهمة.");
//         return;
//     }
//     if (!data.priority || !data.difficulty || !data.expectedMinutes) {
//         showToast("يرجى استكمال الحقول الإلزامية.");
//         return;
//     }

//     showToast("جارٍ حفظ المهمة...");

//     await new Promise(r => setTimeout(r, 450));

//     const newTask = {
//         id: Math.floor(Math.random() * 10000),
//         title: data.title,
//         description: data.description || null,
//         priority: data.priority,
//         difficulty: data.difficulty,
//         expectedMinutes: data.expectedMinutes,
//         dueDate: data.dueDate,
//         status: "pending"
//     };

//     appState.dashboard.todayTasks.unshift(newTask);

//     modalAdd.classList.remove("show");
//     renderDashboard();

//     loadMirrorLightUpdateAfterAdd();
//     showToast("تمت إضافة المهمة بنجاح ✅");
// });

// function loadMirrorLightUpdateAfterAdd() {
//     if ((appState.mirror?.totalTasks ?? 0) === 0) {
//         appState.mirror = {
//             ...appState.mirror,
//             totalTasks: 1,
//             doneTasks: 0,
//             doneRatePct: 0,
//             xp: 0,
//             mostFailReason: "—",
//             hardestDoneTitle: "—",
//             motivation: "ابدأ بإحراز تقدم اليوم، وستظهر لك المرآة تدريجيًا."
//         };
//     }
//     renderMirror();
// }

// // =============================
// // Complete Task modal save
// // =============================
// function openComplete(taskId) {
//     $("#completeTaskId").value = taskId;
//     $("#actualMinutes").value = 25;
//     $("#completeNote").value = "";

//     modalComplete.classList.add("show");
//     $("#confetti").style.display = "none";
// }

// $("#saveComplete").addEventListener("click", async () => {
//     const taskId = Number($("#completeTaskId").value);
//     const actual = Number($("#actualMinutes").value);
//     const note = $("#completeNote").value.trim();

//     if (!actual || actual < 1) {
//         showToast("يرجى إدخال الوقت الفعلي (بالدقائق).");
//         return;
//     }

//     showToast("جارٍ تسجيل الإنجاز...");

//     await new Promise(r => setTimeout(r, 500));

//     const tasks = appState.dashboard.todayTasks;
//     const t = tasks.find(x => x.id === taskId);
//     if (t) {
//         t.status = "done";
//         t.actualMinutes = actual;
//         t.note = note || null;
//     }

//     const conf = $("#confetti");
//     conf.style.display = "block";
//     $("#celebrateBox").style.borderColor = "rgba(34,197,94,.55)";
//     showToast("مبروك! تم تسجيل الإنجاز 🎉");

//     modalComplete.classList.remove("show");

//     renderDashboard();
//     updateMirrorAfterDone(taskId);
// });

// function updateMirrorAfterDone(taskId) {
//     if (!appState.mirror) return;
//     const has = (appState.mirror.totalTasks ?? 0) > 0;
//     if (!has) {
//         appState.mirror.totalTasks = appState.dashboard.todayTasks.length;
//         appState.mirror.doneTasks = appState.dashboard.todayTasks.filter(t => t.status === "done").length;
//     } else {
//         appState.mirror.doneTasks = (appState.mirror.doneTasks ?? 0) + 1;
//         appState.mirror.totalTasks = (appState.mirror.totalTasks ?? 0) + 0;
//     }
//     appState.mirror.doneRatePct = Math.round(((appState.mirror.doneTasks ?? 0) / Math.max(1, appState.mirror.totalTasks ?? 1)) * 100);
//     appState.mirror.xp = (appState.mirror.xp ?? 0) + 20;
//     appState.mirror.motivation = "👏 ممتاز! استمر على هذا النمط وقلّل العوائق فور ظهورها.";
//     renderMirror();
// }

// // =============================
// // Fail modal save
// // =============================
// function openFail(taskId) {
//     $("#failTaskId").value = taskId;
//     $("#failExtra").value = "";
//     modalFail.classList.add("show");
// }

// $("#saveFail").addEventListener("click", async () => {
//     const taskId = Number($("#failTaskId").value);
//     const reason = $("#failForm input[name='reason']:checked")?.value;
//     const extra = $("#failExtra").value.trim();

//     if (!reason) {
//         showToast("يرجى اختيار سبب.");
//         return;
//     }

//     showToast("جارٍ إرسال سبب عدم الإنجاز...");

//     await new Promise(r => setTimeout(r, 450));

//     const tasks = appState.dashboard.todayTasks;
//     const t = tasks.find(x => x.id === taskId);
//     if (t) {
//         t.status = "fail";
//         t.failReason = reason;
//         t.failNote = extra || null;
//     }

//     modalFail.classList.remove("show");
//     renderDashboard();
//     updateMirrorAfterFail(reason);
//     showToast("تم تسجيل سبب الفشل ✖️");
// });

// function reasonToArLabel(reason) {
//     switch (reason) {
//         case "forgot": return "نسيت";
//         case "no_time": return "لم يكن لدي وقت";
//         case "harder_than_expected": return "كانت أصعب مما توقعت";
//         case "lost_motivation": return "فقدت الحماس";
//         case "other": return "سبب آخر";
//         default: return "—";
//     }
// }

// function updateMirrorAfterFail(reason) {
//     if (!appState.mirror) return;
//     appState.mirror.mostFailReason = "😓 " + reasonToArLabel(reason);

//     const rate = appState.mirror.doneRatePct ?? 0;
//     if (rate < 60) appState.mirror.motivation = "🌧️ يبدو أن العوائق ظاهرة. جرّب تقليل حجم المهام أو تحديد وقت بداية ثابت.";
//     else appState.mirror.motivation = "تحسن جيد — واصل، وراقب أسباب الفشل لتفادي تكرارها.";
//     renderMirror();
// }

// // =============================
// // Task Details modal (Screen 6)
// // =============================
// function openDetails(taskId) {
//     const t = appState.dashboard.todayTasks.find(x => x.id === taskId);
//     if (!t) return;

//     $("#dTitle").textContent = t.title || "—";
//     $("#dDesc").textContent = t.description || "لا يوجد وصف";
//     $("#dPriority").textContent = priorityToBadge(t.priority);
//     $("#dDifficulty").textContent = difficultyToBadge(t.difficulty);
//     $("#dExpected").textContent = `${t.expectedMinutes ?? 0} دقيقة`;

//     if (t.status === "done") {
//         $("#dActual").textContent = `${t.actualMinutes ?? "—"} دقيقة`;
//     } else {
//         $("#dActual").textContent = "—";
//     }

//     const st = t.status;
//     const stText = st === "pending" ? "قيد التنفيذ / في الانتظار" : (st === "done" ? "منجزة" : "فاشلة");
//     $("#dStatus").textContent = stText;

//     const slot = $("#dImageSlot");
//     slot.innerHTML = "";
//     if (t.imageUrl) {
//         const img = document.createElement("img");
//         img.src = t.imageUrl;
//         img.alt = "صورة المهمة";
//         img.style.maxWidth = "100%";
//         img.style.maxHeight = "160px";
//         slot.appendChild(img);
//     } else {
//         slot.textContent = "لا توجد صورة";
//     }

//     $("#modalDetailsSub").textContent = `المهمة رقم: ${t.id}`;

//     const editBtn = $("#btnEditTask");
//     const delBtn = $("#btnDeleteTask");
//     const doneBtn = $("#btnDetailsDone");

//     doneBtn.style.display = (t.status === "pending") ? "inline-flex" : "none";

//     doneBtn.onclick = () => openComplete(t.id);

//     editBtn.onclick = () => showToast("تعديل المهمة: جاهز للربط بالـ API لاحقاً.");
//     delBtn.onclick = async () => {
//         if (!confirm("هل تريد حذف هذه المهمة؟")) return;
//         showToast("جارٍ حذف المهمة...");
//         await new Promise(r => setTimeout(r, 450));

//         appState.dashboard.todayTasks = appState.dashboard.todayTasks.filter(x => x.id !== t.id);
//         renderDashboard();
//         modalDetails.classList.remove("show");
//         showToast("تم حذف المهمة ✅");
//     };

//     $("#detailsHint").textContent = t.status === "pending"
//         ? "يمكنك تعديل، حذف، أو إنهاء المهمة."
//         : (t.status === "done" ? "تمت المهمة — يمكنك حذف/تعديل (حسب سياسة المشروع)." : "فشلت المهمة — يمكنك حذف/تعديل.");

//     modalDetails.classList.add("show");
// }

// // =============================
// // Start new week
// // =============================
// $("#btnStartNewWeek").addEventListener("click", async () => {
//     if (!confirm("هل تريد بدء أسبوع جديد؟ سيتم تصفير البيانات الحالية (حسب سياسة المشروع).")) return;

//     showToast("جارٍ بدء أسبوع جديد...");
//     await new Promise(r => setTimeout(r, 600));

//     appState.dashboard.week = { progressPct: 0, targetPct: 80, xp: 0, tasksTotal: 0, statusText: "ابدأ من جديد!" };
//     appState.dashboard.todayTasks = [];
//     appState.mirror = { totalTasks: 0, doneTasks: 0, doneRatePct: 0, xp: 0, mostFailReason: "—", hardestDoneTitle: "—", motivation: "ابدأ بإضافة مهام وستظهر مرآتك فورًا." };

//     renderDashboard();
//     renderMirror();
//     showToast("تم بدء الأسبوع الجديد ✅");
// });

// // For tile sync on user click
// ["priorityTiles", "difficultyTiles", "failReasonsTiles"].forEach(id => {
//     const root = $("#" + id);
//     if (!root) return;
//     root.addEventListener("click", (e) => {
//         const tile = e.target.closest(".tile");
//         if (!tile) return;
//         const input = tile.querySelector("input");
//         if (!input) return;
//         const name = input.name;
//         const groupInputs = $$(`input[name="${name}"]`);
//         groupInputs.forEach(x => x.checked = false);
//         input.checked = true;

//         root.querySelectorAll(".tile").forEach(t => {
//             const inp = t.querySelector("input");
//             t.classList.toggle("active", inp && inp.checked);
//         });
//     });

//     root.querySelectorAll(".tile").forEach(t => {
//         const inp = t.querySelector("input");
//         t.classList.toggle("active", inp && inp.checked);
//     });
// });

// syncTiles();