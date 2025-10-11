Work Orders Directive
=====================

Location
--------
- Base directory: `docs/work-orders/`
- Active order details: `docs/work-orders/in-progress/<ORDER>.md`
- Kanban board: `docs/work-orders/board.md`

Workflow
--------
1. When an order becomes active, create/update the detail file in `in-progress/`.
2. Move the item across the Kanban table columns in `board.md` (Ready → In Progress → Code Review → Testing → Done). Use Markdown links to the detail file when possible.
3. For blocked tasks, place the order in the “Blocked” column and note the reason in the detail file.
4. On completion, update the detail file with outcomes (tests, commits) and move the order to “Done”.
5. Keep the board’s date current when major updates occur.

Responsibilities
----------------
- Ensure every new order has: context, tasks, acceptance criteria, and links to reference docs.
- Maintain traceability by referencing PRs or commits inside the order detail file when available.
